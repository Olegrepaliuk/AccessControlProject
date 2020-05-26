using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControl.ViewModels;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private string baseAddress = "https://localhost:44381/identity/users";
        private HttpClient client;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var obj = new
                {
                    UserName = model.Email,
                    FullName = model.FullName,
                    Password = model.Password,
                    Role = model.IsAdmin ? "Admin" : "User"
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var response = await client.PostAsJsonAsync(baseAddress, obj);
                return RedirectToAction("Index");
            }

            return View(model);         
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            string stringId = id.ToString();
            var response = await client.DeleteAsync($"{baseAddress}/{stringId}");
            return RedirectToAction("Index");
        }
    }
}