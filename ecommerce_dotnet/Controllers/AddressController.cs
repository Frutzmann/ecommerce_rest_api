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
    public class AddressController : Controller
    {

        private readonly APIDbContext _context;
        private readonly Security.IJwtUtils _jwt;
        private readonly IAddressRepository _addressRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public AddressController(APIDbContext context, Security.IJwtUtils jwt, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwt = jwt;
            _addressRepository = new AddressRepository(_context);
            _httpContextAccessor = httpContextAccessor;
        }

        [Security.Authorize]
        [HttpPost("/address")]
        public async Task<IActionResult> CreateAddress([FromBody] Address address)
        {
            var user = await GetCurrentUser();
            var query = _addressRepository.CreateAddress(new Address()
            {
                road = address.road,
                postalCode = address.postalCode,
                city = address.city,
                country = address.country,
                User = user,
                updatedDate = DateTime.Now,
            });

            return StatusCode(201, query);  
        }

        [Security.Authorize]
        [HttpGet("/address")]
        public async Task<IActionResult> GetAddresses()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var list = _addressRepository.GetAddress();

            if (!IsCurrentUserAdmin(user))
            {
                list = list.FindAll(a => a.User == user);
            }

            return StatusCode(StatusCodes.Status200OK, list);
        }

        [Security.Authorize]
        [HttpGet("/address/{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var query = _addressRepository.GetAddressById(id);
            if (query == null)
                return StatusCode(StatusCodes.Status404NotFound, Json(new{error= "Address not Found" }).Value);

            var user = await GetCurrentUser();
            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }
            if (query.User != user && !IsCurrentUserAdmin(user))
            {
                return StatusCode(StatusCodes.Status403Forbidden, Json(new {error= "You can't access this address." }).Value);

            }
            return StatusCode(200, query); 
        }

        [Security.Authorize]
        [HttpGet("/address/user")]
        public async Task<IActionResult> GetAddressByUser()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var query = _addressRepository.GetAddressByUser(currentUser);
            if (query == null)
                return NotFound(Json(new { Error = "Addresses Not Found" }));

            return Ok(query);
        }


        [Security.Authorize]
        [HttpDelete("/address/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var findAddress = _addressRepository.GetAddressById(id);

            if (findAddress == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, Json(new { error = "Address not Found" }).Value);
            }

            var user = await GetCurrentUser();
            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }
            if (!IsCurrentUserAdmin(user) && findAddress.User != user)
                return StatusCode(StatusCodes.Status403Forbidden, Json(new{error= "You're not allowed to delete this address" }).Value);

            var query = _addressRepository.DeleteAddress(id);
            return StatusCode(StatusCodes.Status200OK, Json(new {success=query}).Value); 
            
        }

        [Security.Authorize]
        [HttpPut("/address/{id}")]
        public async Task<IActionResult> ChangeAddress(int id, [FromBody] Address address)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return Unauthorized(Json(new { Error = "Please provide an auth token" }));
            }

            var currentAddress = _addressRepository.GetAddressById(id);

            if (currentAddress == null)
                return NotFound(Json(new { Error = "Address not found" }));
            
            if (!IsCurrentUserAdmin(user) && user != currentAddress.User)
            {
                return StatusCode(StatusCodes.Status403Forbidden, Json(new {error= "You're not allowed to edit this address" }).Value);
            }

            var query = _addressRepository.UpdateAddress(currentAddress);
            return StatusCode(StatusCodes.Status201Created, query); 
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
