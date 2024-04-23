using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quest_web.Models;
using quest_web.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace quest_web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly APIDbContext _context;
        public  readonly IUserRepository _userRepository;
        private readonly Security.IJwtUtils _jwtTokenUtil;
        private IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(APIDbContext dbContext, Security.IJwtUtils jwtTokenUtil, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _userRepository = new UserRepository(_context);
            _jwtTokenUtil = jwtTokenUtil;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public IActionResult Register([FromBody] User user)
        {
            UserRepository.QueryResult queryResult = _userRepository.AddUser(user);
            switch (queryResult)
            {
                case UserRepository.QueryResult.ALL_FIELDS_NOT_COMPLETE:
                    return BadRequest(Json(new {Error="Username or password is missing. Both are required !" }).Value);
                case UserRepository.QueryResult.USERNAME_TAKEN:
                    return Conflict(Json(new {Error= "Username already exists in our database" }).Value);
                case UserRepository.QueryResult.SUCCESS:
                    UserDetails details = new() { Id = user.Id, Username = user.Username, Role = user.Role };
                    return StatusCode(201, details);
                default:
                    return null;
            }
        }

         [AllowAnonymous]
         [HttpPost("/authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            UserDetails userToAuth = new UserDetails { Username = user.Username, Role=user.Role };

            try
            {
                var currentUser = _context.User.FirstOrDefault(u => u.Username == user.Username);
                if (BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password) == false)
                {
                    return Unauthorized(Json(new { Error = "Username and password does not match. Please try again." }).Value);
                }
                var Token = _jwtTokenUtil.GenerateToken(userToAuth);
                return Ok(Token);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(Json(new { Error = "User not found." }).Value);
            }
        }


        [Security.Authorize]
        [HttpGet("/me")]
        public async Task<IActionResult> Me()
        {
            string jwtToken;
            try
            {
                jwtToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            }
            catch (Exception e)
            {
                jwtToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
                jwtToken = jwtToken.ToString().Replace("Bearer", "").Trim();
            }
            string token = jwtToken.ToString();

            if (token == "")
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var username = _jwtTokenUtil.GetUsernameFromToken(token);


            return Ok(_userRepository.GetUserDetails(username));
        }
    }
}
