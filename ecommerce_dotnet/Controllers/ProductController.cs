using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quest_web.Models;
using quest_web.Repository;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace quest_web.Controllers
{
    public class ProductController : Controller
    {
        private readonly APIDbContext _context;
        private readonly Security.IJwtUtils _jwt;
        private readonly IProductRepository _productRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public ProductController(APIDbContext context, Security.IJwtUtils jwt, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwt = jwt;
            _productRepository = new ProductRepository(_context);
            _httpContextAccessor = httpContextAccessor; 
        }

        [Security.Authorize]
        [HttpGet("/products")]
        public async Task<IActionResult> GetProducts()
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));
            
            var query = _productRepository.GetProducts();

            return Ok(query);
        }

        [Security.Authorize]
        [HttpGet("/products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _productRepository.GetProductById(id);
            
            if(query == null)
                return NotFound(Json(new { Error = "Product not Found"}));

            return Ok(query);
        }

        [Security.Authorize]
        [HttpPost("/products")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        { 
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));
            
            var query = _productRepository.CreateProduct(product);

            return Ok(query); 
        }

        [Security.Authorize]
        [HttpPut("/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null || !IsCurrentUserAdmin(currentUser))
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _productRepository.UpdateProduct(product);
            if (query == null)
                return NotFound(Json(new { Error = "Product Not Found" }));

            return Ok(query);
        }

        [Security.Authorize]
        [HttpDelete("/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            var currentUser = await GetCurrentUser();
            if (currentUser == null || !IsCurrentUserAdmin(currentUser))
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _productRepository.DeleteProduct(id);
            if (query == false)
                return NotFound(Json(new { Error = "Product Not Found" }));

            return Ok(query);

        }

        public async Task<User> GetCurrentUser()
        {
            string jwtToken;
            try
            {
                jwtToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            }
            catch (Exception e)
            {
                jwtToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
                if (jwtToken != null)
                {
                    jwtToken = jwtToken.ToString().Replace("Bearer", "").Trim();
                }
            }

            string token = "";

            if (jwtToken != null)
            {
                token = jwtToken.ToString();
            }


            if (token == "")
            {
                return null;
            }


            var username = _jwt.GetUsernameFromToken(token);


            return _context.User.FirstOrDefault(u => u.Username == username);
        }

        public bool IsCurrentUserAdmin(User isAdminUser)
        {
            return isAdminUser.Role == UserRole.ROLE_ADMIN;
        }
    }
}
