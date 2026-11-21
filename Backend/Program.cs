using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeamLink.Backend.Auth;
using TeamLink.Backend.Common;
using TeamLink.Backend.Conversations;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"];
var conn = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32 || string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException("Configure Jwt__Key (32+ bytes) and ConnectionStrings__Default via environment variables.");
builder.Services.AddDbContext<TeamLinkDb>(x => x.UseNpgsql(conn));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ConversationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT access token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TeamLink",
        ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TeamLinkClients",
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
        ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(10)
    };
    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            if (!Guid.TryParse(ctx.Principal?.FindFirstValue("sid"), out var sid) ||
                !Guid.TryParse(ctx.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var uid))
            { ctx.Fail("Invalid session."); return; }
            var db = ctx.HttpContext.RequestServices.GetRequiredService<TeamLinkDb>();
            if (!await db.Sessions.AnyAsync(s => s.Id == sid && s.UserId == uid && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow))
                ctx.Fail("Session revoked.");
        }
    };
});
builder.Services.AddAuthorization();
var app = builder.Build();
//classroom prototype: creates missing tables in a fresh development database.
// Replace with EF Core migrations before deploying or changing the schema.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<TeamLinkDb>().Database.EnsureCreatedAsync();
}
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (ApiException ex) { ctx.Response.StatusCode = ex.Status; await ctx.Response.WriteAsJsonAsync(new { code = ex.Code, message = ex.Message }); }
    catch (DbUpdateException) { ctx.Response.StatusCode = 409; await ctx.Response.WriteAsJsonAsync(new { code = "DATABASE_CONFLICT", message = "Data conflict." }); }
});
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

static void SetRefreshCookie(HttpContext ctx, string value)
    => ctx.Response.Cookies.Append("teamlink_refresh", value, new CookieOptions { HttpOnly = true, Secure = ctx.Request.IsHttps, SameSite = SameSiteMode.Strict, Path = "/api/auth", Expires = DateTimeOffset.UtcNow.AddDays(30) });

app.MapPost("/api/auth/register", async (RegisterRequest req, AuthService auth) =>
{
    var user = await auth.Register(req);
    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Name, user.Surname, user.Email });
});
app.MapPost("/api/auth/login", async (LoginRequest req, AuthService auth, HttpContext ctx) =>
{
    var (response, refresh) = await auth.Login(req);
    SetRefreshCookie(ctx, refresh);
    return Results.Ok(response);
});
app.MapPost("/api/auth/refresh", async (AuthService auth, HttpContext ctx) =>
{
    if (!ctx.Request.Cookies.TryGetValue("teamlink_refresh", out var token) || string.IsNullOrEmpty(token))
        throw new ApiException(401, "NO_REFRESH", "No refresh token.");
    var (response, refresh) = await auth.Refresh(token);
    SetRefreshCookie(ctx, refresh);
    return Results.Ok(response);
});
app.MapPost("/api/auth/logout", async (AuthService auth, HttpContext ctx) =>
{
    if (!Guid.TryParse(ctx.User.FindFirstValue("sid"), out var sid)) throw new ApiException(401, "UNAUTHORIZED", "Invalid session.");
    await auth.Logout(ctx.User.UserId(), sid);
    ctx.Response.Cookies.Delete("teamlink_refresh", new CookieOptions { Path = "/api/auth" });
    return Results.NoContent();
}).RequireAuthorization();

app.MapGet("/api/users/me", async (HttpContext ctx, TeamLinkDb db) =>
{
    var id = ctx.User.UserId();
    var u = await db.Users.FindAsync(id) ?? throw new ApiException(404, "NOT_FOUND", "User not found.");
    return Results.Ok(new { u.Id, u.Name, u.Surname, u.Email, u.Bio, u.AvatarUrl });
}).RequireAuthorization();
app.MapPatch("/api/users/me", async (HttpContext ctx, TeamLinkDb db, UpdateProfileRequest req) =>
{
    var u = await db.Users.FindAsync(ctx.User.UserId()) ?? throw new ApiException(404, "NOT_FOUND", "User not found.");
    if (req.Name != null) u.Name = Api.Required(req.Name, "Name", 100);
    if (req.Surname != null) u.Surname = Api.Required(req.Surname, "Surname", 100);
    if (req.Bio != null && req.Bio.Length > 500) throw new ApiException(400, "INVALID_BIO", "Bio is too long.");
    if (req.Bio != null) u.Bio = req.Bio;
    await db.SaveChangesAsync();
    return Results.Ok(new { u.Id, u.Name, u.Surname, u.Email, u.Bio });
}).RequireAuthorization();
app.MapGet("/api/users", async (HttpContext ctx, TeamLinkDb db, string? q) =>
{
    var me = ctx.User.UserId();
    if (string.IsNullOrWhiteSpace(q) || q.Length < 2) return Results.Ok(Array.Empty<object>());
    var search = q.Trim().ToLower();
    return Results.Ok(await db.Users.Where(u => u.Id != me &&
        (u.Name.ToLower().Contains(search) || u.Surname.ToLower().Contains(search)))
        .OrderBy(u => u.Name).Take(20).Select(u => new { u.Id, u.Name, u.Surname }).ToListAsync());
}).RequireAuthorization();

// BR-03: private conversations (no messages yet).
app.MapPost("/api/conversations/private", async (HttpContext ctx, ConversationService svc, CreatePrivateConversationRequest req)
    => Results.Ok(await svc.CreatePrivate(ctx.User.UserId(), req))).RequireAuthorization();
app.MapGet("/api/conversations", async (HttpContext ctx, ConversationService svc)
    => Results.Ok(await svc.List(ctx.User.UserId()))).RequireAuthorization();
app.MapGet("/api/conversations/{id:guid}/participants", async (Guid id, HttpContext ctx, ConversationService svc)
    => Results.Ok(await svc.Participants(id, ctx.User.UserId()))).RequireAuthorization();

app.Run();

public record UpdateProfileRequest(string? Name, string? Surname, string? Bio);
