using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC_Demo.Models;
using MVC_Demo.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;

namespace MVC_Demo.Controllers
{
    public class LoginAccount : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
