using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations;
using System;

namespace TestAPI
{
    public class AllTests
    {
        private readonly RegistrationManagement _registrationManagement;
        private readonly Mock<ILogger<RegistrationManagement>> _loggerMock;
        private readonly AppDBContext _db;
        private readonly LoginManagement _loginManagement;
        private readonly Mock<ILogger<LoginManagement>> _loggerLoginMock;
        public AllTests()
        {

            _loggerMock = new Mock<ILogger<RegistrationManagement>>();
            _loggerLoginMock = new Mock<ILogger<LoginManagement>>();


            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
            _db = new AppDBContext(options);
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            var hasher = new Argon2PasswordHasher<User>();

            _registrationManagement = new RegistrationManagement(
                context: _db,
                logger: _loggerMock.Object,
                passwordHasher: hasher
            );

            _loginManagement = new LoginManagement(
                context: _db,
                logger: _loggerLoginMock.Object,
                passwordHasher: hasher);
        }

        private RequestUser CreateTestUser(string email, string password, string userName)
            => new RequestUser
            {
                Email = email,
                Password = password,
                UserName = userName
            };

        [Fact]
        public async Task RegisterUser_ShouldHashPasswordAndStoreIt()
        {
            var testUser = CreateTestUser("test@test.test", "password", "TestUser");

            var result = await _registrationManagement.RegisterUserAsync(testUser);
            var userInDb = await _db.Users_Data.SingleOrDefaultAsync(u => u.Email == testUser.Email);

            Assert.NotNull(userInDb);
            Assert.Equal(201, result.StatusCode);
            Assert.True(result.Success);

            Assert.NotEqual(testUser.Password, userInDb.Password);

            var verify = new Argon2PasswordHasher<User>()
                .VerifyHashedPassword(userInDb!, userInDb!.Password!, testUser.Password!);
            Assert.Equal(PasswordVerificationResult.Success, verify);
        }

        [Fact]
        public async Task RegisterUser_RemoveUser()
        {
            var testUser = CreateTestUser("test@test.test", "password", "TestUser");
            await _registrationManagement.RegisterUserAsync(testUser);

            var result = await _registrationManagement.RemoveUserAsync(testUser.Email);

            var userInDb = await _db.Users_Data.SingleOrDefaultAsync(u => u.Email == testUser.Email);

            Assert.Null(userInDb);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Success);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Data);
        }



        [Fact]
        public async Task LoginUserTestTrue()
        {
            var testUser = CreateTestUser("test@test.test", "password", "TestUser");
            await _registrationManagement.RegisterUserAsync(testUser);

            var services = new ServiceCollection();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = "/login";
                    opts.LogoutPath = "/logout";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    opts.SlidingExpiration = true;
                    opts.Cookie.HttpOnly = true;
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    opts.Cookie.SameSite = SameSiteMode.Lax;
                });

            services.AddLogging(); 
            services.AddHttpContextAccessor(); 

            var provider = services.BuildServiceProvider();


            var context = new DefaultHttpContext
            {
                RequestServices = provider
            };

            // 3. Вызываем Login
            var result = await _loginManagement.LoginUserAsync(context, testUser);

            // 4. Проверки
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
        }

    }

}
