using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeamLink.Backend.Common;

namespace TeamLink.Backend.Auth;
public record RegisterRequest(string Name, string Surname, string Email, string Password, string ConfirmPassword);
public record LoginRequest(string Email, string Password, string Platform = "WEB");
public record TokenResponse(string AccessToken, DateTime AccessExpiresAt, Guid UserId);
public sealed class AuthService(TeamLinkDb db, IConfiguration config)
{
    private static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    public async Task<User> Register(RegisterRequest request)
    {
        var email = Api.Required(request.Email, "Email", 254).ToLowerInvariant();
        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            throw new ApiException(400, "INVALID_EMAIL", "Invalid email.");
        if (request.Password != request.ConfirmPassword || request.Password.Length < 8 ||
            !request.Password.Any(char.IsLetter) || !request.Password.Any(char.IsDigit))
            throw new ApiException(400, "INVALID_PASSWORD", "Password must match confirmation, be at least 8 characters and contain a letter and a digit.");
        if (await db.Users.AnyAsync(u => u.Email == email))
            throw new ApiException(409, "EMAIL_EXISTS", "Email is already registered.");
        var user = new User { Name = Api.Required(request.Name, "Name", 100), Surname = Api.Required(request.Surname, "Surname", 100), Email = email };
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }
    public async Task<(TokenResponse response, string refresh)> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        const string error = "Email or password is incorrect. Please try again.";
        if (user is null) throw new ApiException(401, "INVALID_LOGIN", error);
        if (user.LockedUntil > DateTime.UtcNow) throw new ApiException(429, "LOGIN_LOCKED", "Too many attempts. Try again later.");
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= 5) { user.LockedUntil = DateTime.UtcNow.AddMinutes(15); user.FailedLoginCount = 0; }
            await db.SaveChangesAsync();
            throw new ApiException(401, "INVALID_LOGIN", error);
        }
        if (!new[] { "WEB", "IOS", "ANDROID" }.Contains(request.Platform)) throw new ApiException(400, "INVALID_PLATFORM", "Invalid platform.");
        user.FailedLoginCount = 0; user.LockedUntil = null;
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var session = new Session { UserId = user.Id, RefreshTokenHash = Hash(refresh), Platform = request.Platform, ExpiresAt = DateTime.UtcNow.AddDays(30) };
        db.Sessions.Add(session);
        await db.SaveChangesAsync();
        return (IssueAccess(user, session), refresh);
    }
    public async Task<(TokenResponse response, string refresh)> Refresh(string refreshToken)
    {
        var tokenHash = Hash(refreshToken);
        var session = await db.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == tokenHash);
        if (session is null || session.RevokedAt is not null || session.ExpiresAt <= DateTime.UtcNow)
            throw new ApiException(401, "INVALID_REFRESH", "Refresh token expired or revoked.");
        var user = await db.Users.FindAsync(session.UserId) ?? throw new ApiException(401, "INVALID_REFRESH", "Invalid session.");
        var next = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        session.RefreshTokenHash = Hash(next);
        session.ExpiresAt = DateTime.UtcNow.AddDays(30);
        await db.SaveChangesAsync();
        return (IssueAccess(user, session), next);
    }
    public async Task Logout(Guid userId, Guid sessionId)
    {
        var session = await db.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session is not null) { session.RevokedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
    }
    private TokenResponse IssueAccess(User user, Session session)
    {
        var expires = DateTime.UtcNow.AddMinutes(15);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var jwt = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim("sid", session.Id.ToString())
        }, expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(jwt), expires, user.Id);
    }
}
