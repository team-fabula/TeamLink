using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace TeamLink.Api.Common;

public sealed record ApiErrorResponse(string Code, string Message);

public static class Extensions
{
    
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        // Load typed configuration options and register services here.
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
}