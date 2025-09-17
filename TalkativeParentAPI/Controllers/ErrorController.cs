using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("Error")]
        [HttpGet]
        public IActionResult Error()
        {
            return Ok(new { 
              Response="Internal Server error",
              Statuscode=HttpStatusCode.InternalServerError
            });
        }
    }
}
