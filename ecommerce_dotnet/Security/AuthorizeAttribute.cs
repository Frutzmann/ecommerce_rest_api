using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace quest_web.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymousAuth = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymousAuth)
                return;

            var user = (Models.UserDetails)context.HttpContext.Items["User"];
            if (user == null)
                context.Result = new JsonResult(new { error = "Invalid authentication key." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
        }
        
    }
}
