using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Responses;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces;
using System.Security.Claims;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations
{
    public class LoginManagement : ILoginManagement
    {
        private readonly AppDBContext _context;
        private readonly ILogger<LoginManagement> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public LoginManagement(AppDBContext context, ILogger<LoginManagement> logger,
             IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }


        public async Task<ResponseMessage<ResponseUser>> LoginUserAsync(HttpContext ctx, RequestUser user)
        {
            _logger.LogInformation("Attempting to log in user with email {Email}", user.Email);
            var existingUser = await _context.Users_Data.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser == null)
            {
                _logger.LogWarning("User with email {Email} not found", user.Email);
                return CreateResponse(null, 404, "User not found", false);
            }

            if (existingUser.Password == null)
            {
                _logger.LogWarning("Password is null for user with email {Email}", user.Email);
                return CreateResponse(null, 400, "Password is null", false);
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Password verification failed for user with email {Email}", user.Email);
                return CreateResponse(null, 401, "Invalid password", false);
            }

            _logger.LogInformation("User with email {Email} logged in successfully", user.Email);
            var responseUser = CreateResponseUser(existingUser);

            var claims = new[]
        {
             new Claim(ClaimTypes.Name, existingUser.Name),
             new Claim(ClaimTypes.Email, existingUser.Email),
            new Claim(ClaimTypes.Role, existingUser.Role),
        };
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            );
            await ctx.SignInAsync(principal);

            return CreateResponse(responseUser, 200, "User logged in successfully", true);
        }

        public async Task<ResponseMessage<ResponseUser>> LogoutUserAsync(HttpContext ctx)
        {
            await ctx.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
            return CreateResponse(null, 200, "User logged out successfully", true);
        }



        private User CreateUser(RequestUser requestUser)
        {
            return new User
            {
                Email = requestUser.Email,
                Password = requestUser.Password,
                Name = requestUser.UserName,
                CreatedAt = DateTime.UtcNow
            };
        }

        private ResponseUser CreateResponseUser(User user)
        {
            return new ResponseUser
            {
                Email = user.Email,
                UserName = user.Name
            };
        }


        private ResponseMessage<ResponseUser> CreateResponse(ResponseUser? responseUser, int code,
            string message, bool success)
        {
            return new ResponseMessage<ResponseUser>
            {
                StatusCode = code,
                Message = message,
                Success = success,
                Data = responseUser
            };
        }

    }
}
