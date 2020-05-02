using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private string baseAddress = "https://localhost:44381/identity/users";
        private HttpClient client;
        private readonly UserManager<User> _userManager;
        public UsersController(HttpClient cl)
        {
            client = cl;
        }


        public async Task<IActionResult> Index()
        {
            var allUsersWithRoles = new List<Tuple<User, string>>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Tuple<User, string>>>(json);
                allUsersWithRoles = result.ToList();
            }
            return View(allUsersWithRoles);
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