using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace BudgetTracker.API.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        //private readonly IJwtService _jwtService;
        public AccountController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            
        }

        // .../Account/{id}
        [HttpGet]
        [Route("{id:int}", Name = "GetUser")]
        public async Task<ActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserDetails(id);
            return Ok(user);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestModel model)
        { // case-insensitive
            // take name, dob, email passwd and save it to database
            
                //save to db 
                var response = await _userService.CreateUser(model);
                return CreatedAtRoute("GetUser", new { id = response.Id }, response);
        }

         // ..Account/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestModel loginRequest)
        {
            if (!ModelState.IsValid) return Ok();
            UserLoginResponseModel response = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);
            if (response == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return Unauthorized();
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

            //new { token = _jwtService.GenerateToken(response) }
            return Ok();
      
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToRoute("Login");

        }
    }
}
