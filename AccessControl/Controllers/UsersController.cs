using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var simpleUsers = new List<User>();
            foreach (var user in _userManager.Users)
            {

            }
            return View(_userManager.Users.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var listRoles = await _userManager.GetRolesAsync(user);
                if (listRoles.Where(l => l.ToUpper() == "ADMIN").Count() == 0)
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }
                
            }
            return RedirectToAction("Index");
        }
    }
}