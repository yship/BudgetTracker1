using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetTracker.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public AccountController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequestModel model)
        { // case-insensitive
            // take name, dob, email passwd and save it to database
            if (ModelState.IsValid)
            {
                //save to db 
                var response = await _userService.CreateUser(model);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel loginRequest) {
            if (!ModelState.IsValid) return View();
            UserLoginResponseModel response = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);
            if (response == null) {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, response.Email),
                new Claim(ClaimTypes.Name, response.FullName),
                new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()),
               

            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            
            return View("Index", response);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();


            return LocalRedirect("Login");
        }
    }
}
