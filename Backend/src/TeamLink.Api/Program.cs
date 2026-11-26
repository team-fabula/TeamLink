using TeamLink.Api.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommonServices(builder.Configuration);

var app = builder.Build();

app.UseCommonMiddleware();

app.Run();