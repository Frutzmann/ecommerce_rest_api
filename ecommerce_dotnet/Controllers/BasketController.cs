
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quest_web.Models;
using quest_web.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quest_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : Controller
    {
        private readonly APIDbContext _context;
        private readonly Security.IJwtUtils _jwt;
        private readonly IBasketRepository _basketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketController(APIDbContext context, Security.IJwtUtils jwt, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwt = jwt;
            _basketRepository = new BasketRepository(_context);
            _httpContextAccessor = httpContextAccessor;

        }

        [Security.Authorize]
        [HttpGet("/basket")]
        public async Task<IActionResult> GetUserBasket()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var query = _basketRepository.GetBasketByUserId(currentUser);
            if (query == null)
            {
                return NotFound(Json(new { Error = "Basket not found or does not exist" }));
            }

            return Ok(query);
        }

        [Security.Authorize]
        [HttpPost("/basket")]
        public async Task<IActionResult> CreateBasket([FromBody] Basket basket)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _basketRepository.CreateBasket(new Basket()
            {
                User = currentUser,
                Product = basket.Product,
                quantity = basket.quantity
            });
            return Ok(query);
        }

        [Security.Authorize]
        [HttpDelete("/basket/{id}")]
        public async Task<IActionResult> DeleteBasket(int id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _basketRepository.DeleteBasket(id);
            if (query == false)
                return NotFound(Json(new { Error = "Basket Not Found" }));

            return Ok(query); 
        }

        [Security.Authorize]
        [HttpPut("/basket/order/{id}")]
        public async Task<IActionResult> SetBasketOrderNumber(int id, [FromBody] Basket basket)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var currentBasket = _basketRepository.GetBasketById(id);

            if (currentBasket == null)
                return NotFound(Json(new { Error = "Basket not found" }));

            var query = _basketRepository.SetBasketOrderNumber(basket);
            if (query == false)
                return NotFound(new { Error = "Unable to order basket" });

            return Ok(query);
        }

        [Security.Authorize]
        [HttpGet("/basket/{order}")]
        public async Task<IActionResult> GetBasketByOrderNumber(int order)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _basketRepository.GetBasketByOrderNumber(order); 
            if(query == null)
                return NotFound(new { Error = "Unable to order basket" });

            
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
