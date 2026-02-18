using EduNexus.API.Extensions;
using EduNexus.API.Middlewares;
using EduNexus.DependencyInjection;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Infrastructure.SeedData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This tells ASP.NET Core to use enum string values (not numbers)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterConfiguartion(builder.Configuration);
builder.Services.AddApiVersioningConfig();
builder.Services.OptionsPatternConfig(builder.Configuration);
builder.Services.RegisterJwtAuthenticationConfig(builder.Configuration);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

SerilogExtensions.AddSerilogConfiguration(builder.Host);

// in case, ÚäÏí ÇßÔä ãËáÇ ãÍÊÇÌå ÏæÑ æÕáÇÍíå æÔÑØ, áÇÒã ÇÚãáåÇ ÈÔßá customize
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("MustUserName", policy =>
//    {
//        //policy.RequireRole("SuperAdmin");
//        policy.RequireClaim(Permissions.Type, Permissions.Support.Create);
//        policy.RequireUserName("Ahmed Hejazy");
//    });
//});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    await Seed.InitializeAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   // app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(x =>
    x.AllowAnyMethod()
     .AllowAnyOrigin()
     .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

