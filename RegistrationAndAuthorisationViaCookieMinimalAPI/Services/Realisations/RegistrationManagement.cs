using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Responses;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations
{
    public class RegistrationManagement : IRegistrationManagement
    {
        private readonly AppDBContext _context;
        private readonly ILogger<RegistrationManagement> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RegistrationManagement(AppDBContext context, ILogger<RegistrationManagement> logger,
             IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseMessage<ResponseUser>> RegisterUserAsync(RequestUser user)
        {
           
            var result = await _context.Users_Data.FirstOrDefaultAsync(x=>x.Email == user.Email);

            _logger.LogInformation("Checking if user with email {Email} already exists", user.Email);   

            if (result != null) {
                _logger.LogWarning("User with email {Email} already exists", user.Email);
                return CreateResponse(null, 400, "User already exists", false);
            }
            if (user.Password == null || user.Email == null)
            {
                _logger.LogWarning("Email or password is null for user with email {Email}", user.Email);    
                return CreateResponse(null, 400, "Email or password is null", false);
            }

            var newUser = CreateUser(user);
            _logger.LogInformation("Creating new user with email {Email}", user.Email); 
            newUser.Password = _passwordHasher.HashPassword(newUser, user.Password);

            await _context.Users_Data.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var responseUser = CreateResponseUser(newUser);
            _logger.LogInformation("User with email {Email} created successfully", user.Email);
            return CreateResponse(responseUser, 201, "User created successfully", true);
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
            return new ResponseUser { 
                Email = user.Email, 
                UserName = user.Name };
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

        public async Task<ResponseMessage<ResponseUser>> RemoveUserAsync(string email)
        {

            _logger.LogInformation("Attempting to remove user with email {Email}", email);
            var user = await _context.Users_Data.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return CreateResponse(null, 404, "User not found", false);
            }
            _context.Users_Data.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User with email {Email} removed successfully", email);
            return CreateResponse(CreateResponseUser(user), 200, "User removed successfully", true);
        }
    }
}
