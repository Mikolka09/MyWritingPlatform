using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }
    }
}
