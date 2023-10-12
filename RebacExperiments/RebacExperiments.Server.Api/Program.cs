// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Database:
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

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
