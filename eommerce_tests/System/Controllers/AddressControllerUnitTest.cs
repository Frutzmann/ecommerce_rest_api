using System;
using Microsoft.AspNetCore.Http;
using quest_web.Security;
using quest_web;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using quest_web.Controllers;
using quest_web.Models;
using quest_web.Repository;
using Microsoft.EntityFrameworkCore;

namespace quest_web_tests.System.Controllers;

public class AddressControllerUnitTest : IDisposable
{
    private readonly APIDbContext DbContext;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly IHttpContextAccessor _contextAccessor;

    public AddressControllerUnitTest()
    {
        // Setup
        _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

        var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests_address");
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

        foreach (var myAddress in mockedAddresses.MyAddresses)
        {
            DbContext.Set<Address>().Add(myAddress);
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
    public void GetAddresses__WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var addressController = new AddressController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        addressController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)addressController.GetAddresses().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public void GetAddresses__WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var addressController = new AddressController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        addressController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)addressController.GetAddresses().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public void CreateAddress__WithAuthentication_ShouldReturn201()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var addressController = new AddressController(DbContext, _jwtTokenUtil, _contextAccessor);
        var addressToCreate = new Address()
            { id = 55, road = "Avenue des Champs Elysées", postalCode = "75008", city = "Paris 08", country = "France" };

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        addressController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)addressController.CreateAddress(addressToCreate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
    }

    [Fact]
    public void RemoveAddress_WithUser_ShouldReturn403()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 4, Username = "BryanSimon4", Password = "Bryan", Role = UserRole.ROLE_USER };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var addressController = new AddressController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        addressController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)addressController.DeleteAddress(1).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public void RemoveAddress_WithAdmin_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var addressController = new AddressController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        addressController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)addressController.DeleteAddress(2).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }
}