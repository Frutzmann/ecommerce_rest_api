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

public class OrderControllerUnitTest: IDisposable
{
    private readonly APIDbContext DbContext;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly IHttpContextAccessor _contextAccessor;

    public OrderControllerUnitTest()
    {
        // Setup
        _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

        var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests_address");
        dbContextOptions.EnableSensitiveDataLogging();
        DbContext = new APIDbContext(dbContextOptions.Options);
        DbContext.Database.EnsureCreated();

        var mockedUsers = new MockData.MockedUsers();
        var mockedOrders = new MockData.MockedOrders();
        var mockedAddresses = new MockData.MockedAddresses();

        foreach (var myUser in mockedUsers.MyUsers)
        {
            myUser.Password = BCrypt.Net.BCrypt.HashPassword(myUser.Password);
            DbContext.Set<User>().Add(myUser);
        }

        foreach (var myOrders in mockedOrders.MyOrders)
        {
            if(myOrders.user == null)
                myOrders.user = mockedUsers.MyUsers[1]; 
            if(myOrders.address == null)
                myOrders.address = mockedAddresses.MyAddresses[1];

            DbContext.Set<Order>().Add(myOrders);
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
    public void GetOrders__WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)orderController.GetOrders().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void GetOrders__WithAuthenticationAsSimpleUser_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 4, Username = "BryanSimon4", Password = "Bryan", Role = UserRole.ROLE_USER };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)orderController.GetOrders().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void GetOrders_WithAuthenticationAsAdminUser_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);
        

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)orderController.GetOrders().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void CreateOrder_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        var orderToCreate = new Order()
        {
            id = 99,
            address = new Address { id = 1, road = "6 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
            user = userToAuthenticate,
            totalPrice = 1350
        };

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)orderController.CreateOrder(orderToCreate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void CreateOrder_WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);
        var orderToCreate = new Order()
        {
            id = 99,
            address = new Address { id = 2, road = "7 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
            user = userToAuthenticate,
            totalPrice = 1350
        };

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)orderController.CreateOrder(orderToCreate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void RemoveOrder_WithAdmin_ShouldReturn200()
    {
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)orderController.DeleteOrder(2).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void RemoveOrder_WithoutAuthentication_ShouldReturn401()
    {
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User()
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_USER };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)orderController.DeleteOrder(1).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void GetOrderById_WithAuth_ShouldReturn200()
    {
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var orderController = new OrderController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        orderController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)orderController.GetOrderById(3).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}