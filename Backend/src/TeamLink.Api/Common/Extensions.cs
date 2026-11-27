using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Data;

namespace TeamLink.Api.Common;

public sealed record ApiErrorResponse(string Code, string Message);

public static class Extensions
{
    
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static WebApplication UseCommonMiddleware(this WebApplication app)
    {
        app.MapControllers();
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = new ApiErrorResponse(
                    Code: "INTERNAL_ERROR",
                    Message: app.Environment.IsDevelopment() && feature?.Error is not null
                        ? feature.Error.Message
                        : "An unexpected error occurred.");

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            });
        });

        return app;
    }

    public static void HasEnumCheckConstraint<TEntity, TEnum>(this EntityTypeBuilder<TEntity> builder, string columnName)
        where TEntity : class
        where TEnum : struct, Enum
    {
        var tableName = builder.Metadata.GetTableName();
        var allowedValues = string.Join(", ", Enum.GetNames<TEnum>().Select(v => $"'{v}'"));
        var constraintName = $"CK_{tableName}_{columnName}";
        var sql = $"\"{columnName}\" IN ({allowedValues})";

        builder.ToTable(table => table.HasCheckConstraint(constraintName, sql));
    }
}