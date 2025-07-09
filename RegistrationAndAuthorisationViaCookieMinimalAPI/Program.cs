using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLogging();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseNpgsql(connectionString);
});


builder.Services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();
builder.Services.AddScoped<IRegistrationManagement, RegistrationManagement>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
