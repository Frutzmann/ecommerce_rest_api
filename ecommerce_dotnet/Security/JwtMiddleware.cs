using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace quest_web.Security
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, Repository.IUserRepository userRepo, Security.IJwtUtils jwtTokenUtil)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                try
                {
                    var userId = jwtTokenUtil.GetUsernameFromToken(token);
                    if (userId != null)
                    {
                        context.Items["User"] = userRepo.GetUserDetails(userId);
                    }
                } catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            await _next(context);
        }
    }
}
