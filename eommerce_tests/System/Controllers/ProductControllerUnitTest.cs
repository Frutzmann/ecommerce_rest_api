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

public class ProductControllerUnitTest : IDisposable
{
    private readonly APIDbContext DbContext;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly IHttpContextAccessor _contextAccessor;

    public ProductControllerUnitTest()
    {
        // Setup
        _jwtTokenUtil = new Mock<JwtTokenUtil>().Object;

        var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests_address");
        dbContextOptions.EnableSensitiveDataLogging();
        DbContext = new APIDbContext(dbContextOptions.Options);
        DbContext.Database.EnsureCreated();

        var mockedUsers = new MockData.MockedUsers();
        var mockedProducts = new MockData.MockedProducts();

        foreach (var myUser in mockedUsers.MyUsers)
        {
            myUser.Password = BCrypt.Net.BCrypt.HashPassword(myUser.Password);
            DbContext.Set<User>().Add(myUser);
        }

        foreach (var myProducts in mockedProducts.MyProducts)
        {
            DbContext.Set<Product>().Add(myProducts);
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
    public void GetProducts__WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var productController = new ProductController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        productController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = (ObjectResult)productController.GetProducts().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void GetProducts__WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var productController = new ProductController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        productController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)productController.GetProducts().Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void CreateProduct__WithAuthentication_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
            { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var productController = new ProductController(DbContext, _jwtTokenUtil, _contextAccessor);
        var productToCreate = new Product()
            { id = 55, title = "Samsung Galaxy Z Fold 4", description = "Sous tous les angles", price = 2000, currency = "€" };

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        productController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)productController.CreateProduct(productToCreate).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void RemoveAddress_WithUser_ShouldReturn401()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 4, Username = "BryanSimon4", Password = "Bryan", Role = UserRole.ROLE_USER };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var productController = new ProductController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        productController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)productController.DeleteProduct(1).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void RemoveAddress_WithAdmin_ShouldReturn200()
    {
        // Arrange
        var userRepo = new UserRepository(DbContext);
        var userToAuthenticate = new User
        { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN };
        var userDetails = userRepo.GetUserDetails(userToAuthenticate.Username);
        var productController = new ProductController(DbContext, _jwtTokenUtil, _contextAccessor);

        // Authenticate
        var token = _jwtTokenUtil.GenerateToken(userDetails);
        var claims = _jwtTokenUtil.DecodeToken(token);
        productController.ControllerContext.HttpContext = new DefaultHttpContext();
        _contextAccessor.HttpContext.User = claims.Item1;
        _contextAccessor.HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

        // Act
        var result = (ObjectResult)productController.DeleteProduct(3).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}