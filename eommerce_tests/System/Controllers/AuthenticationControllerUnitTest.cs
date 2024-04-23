using quest_web.Controllers;
using quest_web.Security;
using quest_web;
using Moq;
using quest_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using quest_web.Repository;
using quest_web_tests.Attributes;
using quest_web_tests.Helpers;

namespace quest_web_tests.System.Controllers;

[TestCaseOrderer("quest_web_tests.Orderers.PriorityOrderer", "quest_web_tests")]
public class AuthenticationControllerUnitTest : IClassFixture<DatabaseFixture>
{
    private readonly APIDbContext DbContext;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly IHttpContextAccessor _contextAccessor;
   private readonly DatabaseFixture fixture;

    public AuthenticationControllerUnitTest(DatabaseFixture fixture)
    {
        // Setup
        _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

        this.fixture = fixture;
        DbContext = this.fixture.Context;

        _contextAccessor = new Mock<HttpContextAccessor>().Object;
        _contextAccessor.HttpContext = new DefaultHttpContext();
    }

    [Fact, TestPriority(-5)]
    public void CreateUser_ReturnUserObject()
    {
        // Arrange
        var userToAdd = new User { Id = 6, Username = "BryanSimon10", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var authenticationController = new AuthenticationController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Act
        var result = (ObjectResult)authenticationController.Register(userToAdd);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.IsType<UserDetails>(result.Value);
    }

    [Fact, TestPriority(0)]
    public void CreateUser_ShouldReturn409Conflict()
    {
        // Arrange
        var userToAdd = new User { Id = 7, Username = "BryanSimon10", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var authenticationController = new AuthenticationController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Act
        var result = (ObjectResult)authenticationController.Register(userToAdd);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.IsNotType<UserDetails>(result.Value);

    }

    [Fact, TestPriority(10)]
    public void AuthenticateUser_ShouldReturnTokenAndStatus200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
            var authenticationController = new AuthenticationController(DbContext, _jwtTokenUtil, _contextAccessor);
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);

        // Act
        var result = (ObjectResult)authenticationController.Authenticate(userToAuthenticate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(_jwtTokenUtil.GenerateToken(userDetails), result.Value.ToString().Split("=")[1].Replace("}", "").Trim());
    }

    [Fact, TestPriority(15)]
    public void UserAuthenticated__GetMePage_ShouldReturnSuccessAndUserInfo()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var authenticationController = new AuthenticationController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        authenticationController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization",$"Bearer {token}");

        // Act
        var result = (ObjectResult)authenticationController.Me().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.IsType<UserDetails>(result.Value);
    }
}