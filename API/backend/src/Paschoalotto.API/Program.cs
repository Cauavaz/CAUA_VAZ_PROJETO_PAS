using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Paschoalotto.API.Extensions;
using Paschoalotto.Infrastructure;
using Paschoalotto.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddMvc().AddApplicationPart(typeof(Program).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"] ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins.Split(','))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => allowedOrigins.Split(',').Contains(origin))
              .WithExposedHeaders("*");
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

app.Run();