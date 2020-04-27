using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControl.ViewModels;
using AccessControlModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;

namespace AccessControl.Controllers
{
    public class AccountController : Controller
    {
        private AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        private async Task<IActionResult> Register(RegisterViewModel model)
        {
            /*
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, FullName = model.FullName};
                //var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = new IdentityRole();
                    role.Name = "Admin";
                    //await _rolemanager.CreateAsync(role);
                    await _userManager.AddToRolesAsync(user, new List<string>{"Admin"});
                    await _signInManager.SignInAsync(user, false);// setting cookies
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            */
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token =  await _accountService.GetAuthenticationToken(model.Email, model.Password);
                if(!string.IsNullOrEmpty(token))
                {
                    var authProperties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.Now.AddDays(30),
                        IsPersistent = true 
                    };
                    var claims = _accountService.GetClaimsPrincipal(token);
                    ClaimsIdentity id = new ClaimsIdentity(claims.Claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    CookieOptions cookieOptions = new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = false,
                        IsEssential = true,
                        Expires = DateTime.Now.AddMonths(1),
                    };
                    HttpContext.Response.Cookies.Append("token", token, cookieOptions);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), authProperties);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("token");
            return RedirectToAction("Login");
        }
    }
}