using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public AllTests()
        {

            _loggerMock = new Mock<ILogger<RegistrationManagement>>();


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
    }

}
