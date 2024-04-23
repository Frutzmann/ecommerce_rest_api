using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using quest_web.Repository;
using quest_web.Models;
using Microsoft.AspNetCore.Authentication;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace quest_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        // GET: /<controller>/
        private readonly APIDbContext _context;
        private readonly Security.IJwtUtils _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;

        public OrderController(APIDbContext context, Security.IJwtUtils jwtUtils, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwt = jwtUtils;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = new OrderRepository(_context);
        }

        [Security.Authorize]
        [HttpGet("/order")]
        public async Task<IActionResult> GetOrders()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null || !IsCurrentUserAdmin(currentUser))
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _orderRepository.GetOrders();

            return Ok(query);
        }

        [Security.Authorize]
        [HttpPost("/order")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));
            var orderNumber = GenerateOrderNumber();
            order.user = currentUser;
            order.orderNumber = orderNumber;
            var query = _orderRepository.CreateOrder(order);
            if (query == null)
                return BadRequest(Json(new { Error = "Bad Request" })); 

            
            return Ok(orderNumber);
        }

        [Security.Authorize]
        [HttpDelete("/order/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null || !IsCurrentUserAdmin(currentUser))
                return Unauthorized(Json(new { Error = "Access Denied" }));
            var query = _orderRepository.DeleteOrder(id); 
            if(query == false)
                return NotFound(Json(new { Error = "Order Not Found"}));
            return Ok(query);

        }

        [Security.Authorize]
        [HttpGet("/order/user")]
        public async Task<IActionResult> GetOrderByUser()
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _orderRepository.GetOrdersByUser(currentUser);

            if (query == null)
                return NotFound(Json(new { Error = "Ordred not found" })); 

            return Ok(query);
        }

        [Security.Authorize]
        [HttpGet("/order/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null)
                return Unauthorized(Json(new { Error = "Access Denied" }));

            var query = _orderRepository.GetOrderById(id);

            if (query == null)
                return NotFound(Json(new { Error = "Ordred not found" }));

            return Ok(query);
        }

        private int GenerateOrderNumber()
        {
            var rand = new Random();
            return rand.Next(10000, 100000); 
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
