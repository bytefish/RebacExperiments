// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IUserTaskService, UserTaskService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ApplicationDatabase");

    if (connectionString == null)
    {
        throw new InvalidOperationException("No ConnectionString named 'ApplicationDatabase' was found");
    }

    options
        .EnableSensitiveDataLogging().UseSqlServer(connectionString);
});

// Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // We don't want to deal with CSRF Tokens
        
        options.Events.OnRedirectToAccessDenied = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            return Task.CompletedTask;
        };

        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return Task.CompletedTask;
        };
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.RequireUserRole, policy => policy.RequireRole(Roles.User));
    options.AddPolicy(Policies.RequireAdminRole, policy => policy.RequireRole(Roles.Administrator));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
