using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PeopleController : Controller
    {
        static HttpClient client;
        //private static HttpClient client2 = new HttpClient();

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public PeopleController(HttpClient cl, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> rolemanager)
        {
            //client.BaseAddress = new Uri("https://localhost:44330/");
            client = cl;
            _userManager = userManager;
            _signInManager = signInManager;
            _rolemanager = rolemanager;
        }

        private string baseAdress = "https://localhost:44330/api/people";

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            List<Person> allPeople = new List<Person>();
            //var response = await Task.Run(()=>client.GetAsync($"api/people"));
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                var people = await response.Content.ReadAsAsync<IEnumerable<Person>>();
                allPeople = people.ToList(); 
            }
            return View(allPeople);
        }

        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            //var response = await client.DeleteAsync($"api/people/{id}");

            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Delete,
                    uri: baseAdress+"/"+id,
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;

        }
        
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            //var response = await client.PostAsJsonAsync($"api/people", person);
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessageWithObj
                (
                    method: HttpMethod.Post,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: currUser.PasswordHash,
                    obj: person
                );
            var response = await client.SendAsync(message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            //var response = await client.GetAsync($"api/people/{id}");
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress + "/" + id,
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                var person = await response.Content.ReadAsAsync<Person>();
                return View(person);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Person person)
        {
            //var response = await client.PutAsJsonAsync($"api/people/{person.Id}", person);
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessageWithObj
                (
                    method: HttpMethod.Put,
                    uri: baseAdress + "/" + person.Id,
                    username: currUser.UserName,
                    password: currUser.PasswordHash,
                    obj: person
                );

            var response = await client.SendAsync(message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Test2()
        {

            int a = 0;
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );

            var resp = await client.SendAsync(message);
            return View("Index");
        }

    }
}