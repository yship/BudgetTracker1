using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.MVC.Controllers
{
    public class HomeController : Controller
    {
        //localhost/home/index
        public IActionResult Index()
        {
            return View();
        }

        //localhost/home/privacy
        public IActionResult Privacy() {
            return View();
        }

    }
}
