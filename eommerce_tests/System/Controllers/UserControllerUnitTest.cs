using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using quest_web.Controllers;
using quest_web.Models;
using quest_web.Repository;
using quest_web.Security;
using quest_web;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace quest_web_tests.System.Controllers
{
    
    public class UserControllerUnitTest : IDisposable
    {
        private readonly APIDbContext DbContext;
        private readonly JwtTokenUtil _jwtTokenUtil;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserControllerUnitTest()
        {
            // Setup
            _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

            var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests_users");
            dbContextOptions.EnableSensitiveDataLogging();
            DbContext = new APIDbContext(dbContextOptions.Options);
            DbContext.Database.EnsureCreated();

            var mockedUsers = new MockData.MockedUsers();
            var mockedAddresses = new MockData.MockedAddresses();

            foreach (var myUser in mockedUsers.MyUsers)
            {
                myUser.Password = BCrypt.Net.BCrypt.HashPassword(myUser.Password);
                DbContext.Set<User>().Add(myUser);
            }

            DbContext.SaveChanges();

            _contextAccessor = new Mock<HttpContextAccessor>().Object;
            _contextAccessor.HttpContext = new DefaultHttpContext();
        }
        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void GetUsers__WithoutAuthentication_ShouldReturn401()
        {
            // Arrange
            var userRepo = new UserRepository(DbContext);
            var userToAuthenticate = new User
                { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
            var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
            var userController = new UserController(DbContext, _jwtTokenUtil, _contextAccessor);

            // Authenticate
            var token = _jwtTokenUtil.GenerateToken(userDetails);
            userController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = (ObjectResult)userController.GetUsers().Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        }

        [Fact]
        public void GetUsers__WithAuthentication_ShouldReturn200()
        {
            // Arrange
            var userRepo = new UserRepository(DbContext);
            var userToAuthenticate = new User
                { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
            var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
            var userController = new UserController(DbContext, _jwtTokenUtil, _contextAccessor);

            // Authenticate
            var token = _jwtTokenUtil.GenerateToken(userDetails);
            var claims = _jwtTokenUtil.DecodeToken(token);
            userController.ControllerContext.HttpContext = new DefaultHttpContext();
            _contextAccessor.HttpContext.User = claims.Item1;
            _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var result = (ObjectResult)userController.GetUsers().Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void RemoveUser_WithUser_ShouldReturn403()
        {
            // Arrange
            var userRepo = new UserRepository(DbContext);
            var userToAuthenticate = new User
                { Id = 4, Username = "BryanSimon4", Password = "Bryan", Role = UserRole.ROLE_USER };
                var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
            var userController = new UserController(DbContext, _jwtTokenUtil, _contextAccessor);

            // Authenticate
            var token = _jwtTokenUtil.GenerateToken(userDetails);
            var claims = _jwtTokenUtil.DecodeToken(token);
            userController.ControllerContext.HttpContext = new DefaultHttpContext();
            _contextAccessor.HttpContext.User = claims.Item1;
            _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var result = (ObjectResult)userController.DeleteUser(1).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }

        [Fact]
        public void RemoveUser_WithAdmin_ShouldReturn200()
        {
            // Arrange
            var userRepo = new UserRepository(DbContext);
            var userToAuthenticate = new User
                { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
            var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
            var userController = new UserController(DbContext, _jwtTokenUtil, _contextAccessor);

            // Authenticate
            var token = _jwtTokenUtil.GenerateToken(userDetails);
            var claims = _jwtTokenUtil.DecodeToken(token);
            userController.ControllerContext.HttpContext = new DefaultHttpContext();
            _contextAccessor.HttpContext.User = claims.Item1;
            _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var result = (ObjectResult)userController.DeleteUser(5).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
