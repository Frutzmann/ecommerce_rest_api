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

public class BasketControllerUnitTest: IDisposable
{
    private readonly APIDbContext DbContext;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly IHttpContextAccessor _contextAccessor;

    public BasketControllerUnitTest()
    {
        // Setup
        _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

        var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests_address");
        dbContextOptions.EnableSensitiveDataLogging();
        DbContext = new APIDbContext(dbContextOptions.Options);
        DbContext.Database.EnsureCreated();

        var mockedUsers = new MockData.MockedUsers();
        var mockedBaskets = new MockData.MockedBaskets();

        foreach (var myUser in mockedUsers.MyUsers)
        {
            myUser.Password = BCrypt.Net.BCrypt.HashPassword(myUser.Password);
            DbContext.Set<User>().Add(myUser);
        }

        foreach (var myBaskets in mockedBaskets.MyBaskets)
        {
            if(myBaskets.User == null)
                myBaskets.User = mockedUsers.MyUsers[1]; 

            DbContext.Set<Basket>().Add(myBaskets);
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
    public void GetUserBasket__WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var basketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        basketController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)basketController.GetUserBasket().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void GetUserBasket__WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 4, Username = "BryanSimon", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var basketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        basketController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)basketController.GetUserBasket().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void CreateBasket_WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var BasketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);
        var product = new Product()
            { id = 55, title = "Samsung Galaxy Z Fold 4", description = "Sous tous les angles", price = 2000, currency = "€" };
        var basketToCreate = new Basket()
        {
            id = 99,
            User = userToAuthenticate,
            Product = product,
            quantity = 1
        };

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        BasketController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)BasketController.CreateBasket(basketToCreate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void RemoveBasket_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var basketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        basketController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)basketController.DeleteBasket(1).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void RemoveBasket_WithAdmin_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var basketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        basketController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)basketController.DeleteBasket(3).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void AddBasketOrderNumber_WithAuthentication_ShouldReturn200()
    {
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var basketController = new BasketController(DbContext, _jwtTokenUtil, _contextAccessor);
        var basketToUpdate = new Basket() { id = 4, quantity = 1, orderNumber = 999999 }; 

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        basketController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)basketController.SetBasketOrderNumber(4, basketToUpdate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}