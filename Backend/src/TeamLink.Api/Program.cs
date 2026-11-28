using TeamLink.Api.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommonServices(builder.Configuration);

var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();
app.UseCommonMiddleware();

app.Run();