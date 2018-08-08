using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpiderAPI.Controllers
{
    [Route("api/user/[action]")]
    public class UserController : Controller
    {
        [HttpPost]
        public JsonResult login([FromBody] dynamic obj)
        {
            //HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            dynamic a = new { code = 20000, data = new { token = "admin" } };
            return Json(a);
        }
    }
}