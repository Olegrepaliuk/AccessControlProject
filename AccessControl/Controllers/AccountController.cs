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

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private AccountService _accountService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> rolemanager, AccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _rolemanager = rolemanager;
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
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, FullName = model.FullName};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = new IdentityRole();
                    role.Name = "Admin";
                    await _rolemanager.CreateAsync(role);
                    //await _userManager.AddToRolesAsync(user, new List<string>{"Admin"});
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
                    var claims = _accountService.GetClaimsPrincipal(token);
                    ClaimsIdentity id = new ClaimsIdentity(claims.Claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
                /*
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    CookieOptions option = new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = false,
                        IsEssential = true, //<- there
                        Expires = DateTime.Now.AddMonths(1),
                    };
                    HttpContext.Response.Cookies.Append("passhash", user.PasswordHash, option);
                    // check if URL belongs to app
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
                */
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // deleting authentification cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await _signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete("passhash");
            return RedirectToAction("Login");
        }
    }
}