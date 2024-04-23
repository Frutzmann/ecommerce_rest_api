using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace quest_web.Controllers
{
    [Authorize]
    public class DefaultController : Controller
    {
        // GET: /testSuccess
        [AllowAnonymous]
        [Route("testSuccess")]
        public IActionResult testSuccess()
        {
            return StatusCode(200, "success");
        }

        // GET: /testNotFound
        [AllowAnonymous]
        [Route("testNotFound")]
        public IActionResult testNotFound()
        {
            return StatusCode(404, "not found");
        }

        // GET: /testError
        [AllowAnonymous]
        [Route("testError")]
        public IActionResult testError()
        {
            return StatusCode(500, "error");
        }


    }
}