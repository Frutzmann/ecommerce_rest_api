using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using quest_web.Models;
using quest_web.Repository;
using quest_web.Security;
namespace quest_web.Controllers
{
    public class UserController : Controller
    {
        private readonly APIDbContext _context;
        private readonly Security.IJwtUtils _jwt;
        private readonly UserRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(APIDbContext context, IJwtUtils jwt, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwt = jwt;
            _repository = new UserRepository(_context);
            _httpContextAccessor = httpContextAccessor; 
        }

        [Security.Authorize]
        [HttpGet("/user")]
        public async Task<IActionResult> GetUsers()
        {
            var user = await GetCurrentUser();
            var list = _repository.GetUsers();

            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            return StatusCode(StatusCodes.Status200OK, list);
        }

        [Security.Authorize]
        [HttpGet("/user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var query = _repository.GetUserById(id);
            if (query == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, Json(new { error = "User not Found" }).Value);
            }

            var user = await GetCurrentUser();

            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            return StatusCode(StatusCodes.Status200OK, query);
        }

        [Security.Authorize]
        [HttpPut("/user/{id}")]
        public async Task<IActionResult> ChangeUser(int id, [FromBody] User userProvided)
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var userToEdit = _repository.GetUserById(id);

            if (!IsCurrentUserAdmin(user) && user != userToEdit)
            {
                return StatusCode(StatusCodes.Status403Forbidden, Json(new { error = "You're not allowed to edit this user" }).Value);
            }

            if (userProvided.Username != null && userToEdit.Username != userProvided.Username)
            {
                // Check if new username is already taken ?
                var isUsed = _context.User.FirstOrDefault(u => u.Username == userProvided.Username);
                if (isUsed != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, Json(new { error = "This username is already taken" }).Value);
                }

                userToEdit.Username = userProvided.Username;
            }

            if (userProvided.Role != null && userToEdit.Role != userProvided.Role)
            {
                // Check if user is admin
                if (!IsCurrentUserAdmin(user))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, Json(new { error = "You're not allowed to edit the ROLE setting" }).Value);
                }
                userToEdit.Role = userProvided.Role;
            }

            var query = _repository.UpdateUser(userToEdit);
            return StatusCode(StatusCodes.Status201Created, query);
        }

        [Security.Authorize]
        [HttpDelete("/user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var findUser = _repository.GetUserById(id);

            if (findUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, Json(new { error = "User not Found" }).Value);
            }

            var user = await GetCurrentUser();

            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }
            if (!IsCurrentUserAdmin(user) && findUser != user)
            {
                return StatusCode(StatusCodes.Status403Forbidden, Json(new { error = "You're not allowed to delete this user" }).Value);
            }

            var query = _repository.DeleteUser(id);
            return StatusCode(StatusCodes.Status200OK, Json(new { success = query }).Value);
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
