using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControl.ViewModels;
using AccessControlModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;

namespace AccessControl.Controllers
{
    public class AccountController : Controller
    {
        private string baseAddress = "https://localhost:44381/api/account/changePassword";
        private static HttpClient client;
        private AccountService _accountService;
        public AccountController(AccountService accountService, HttpClient cl)
        {
            _accountService = accountService;
            client = cl;
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
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var passwordData = new { OldPassword = model.OldPassword, NewPassword = model.NewPassword };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PostAsJsonAsync(baseAddress, passwordData);
            return RedirectToAction("Index");
        }

        [Authorize]
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