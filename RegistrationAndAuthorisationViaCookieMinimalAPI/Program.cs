using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLogging();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.LoginPath = "/login";
        opts.LogoutPath = "/logout";
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        opts.SlidingExpiration = true;
        opts.Cookie.HttpOnly = true;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.None; // в проде — Always
        opts.Cookie.SameSite = SameSiteMode.Lax;
    });


builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();
builder.Services.AddScoped<IRegistrationManagement, RegistrationManagement>();
builder.Services.AddScoped<ILoginManagement, LoginManagement>();



var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.MapPost("/register", async (IRegistrationManagement registrationManagement, RequestUser user) =>
{
    var response = await registrationManagement.RegisterUserAsync(user);
    if (response.StatusCode != 201)
    {
        return Results.StatusCode(response.StatusCode);
    }

    return Results.Ok(response);
});


app.MapPost("/login",async (HttpContext ctx, ILoginManagement loginManagement, RequestUser user) =>
{
    var response = await loginManagement.LoginUserAsync(ctx, user);
    if (response.StatusCode != 200)
    {
        return Results.StatusCode(response.StatusCode);
    }
    return Results.Ok(response);
});


app.MapPost("/logout", async (HttpContext ctx, ILoginManagement loginManagement) =>
{
    var response = await loginManagement.LogoutUserAsync(ctx);
    if (response.StatusCode != 200)
    {
        return Results.StatusCode(response.StatusCode);
    }
    return Results.Ok(response);
});

app.MapGet("/secret", [Authorize("User")] (ClaimsPrincipal user) =>
{

    return Results.Ok($"Привет, {user.Identity.Name}! Это секретная страница.");
});


app.MapGet("/profile", [Authorize] (ClaimsPrincipal user) =>
{
    var responseUser = new ResponseUser
    {
        UserName = user.Identity.Name,
        Email = user.FindFirst(ClaimTypes.Email)?.Value,
    };
    return Results.Ok(responseUser);
});


app.Run();
