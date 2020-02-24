using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AccessControl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PeopleController : Controller
    {
        static HttpClient client;
        //private static HttpClient client2 = new HttpClient();

        public PeopleController(HttpClient cl)
        {
            //client.BaseAddress = new Uri("https://localhost:44330/");
            client = cl;
        }

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
            var response = await Task.Run(()=>client.GetAsync($"api/people"));
            //var response = await Task.Run(() => client2.GetAsync("https://localhost:44330/api/people"));
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
            var response = await client.DeleteAsync($"api/people/{id}");
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
            var response = await client.PostAsJsonAsync($"api/people", person);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var response = await client.GetAsync($"api/people/{id}");
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
            var response = await client.PutAsJsonAsync($"api/people/{person.Id}", person);
            return RedirectToAction("Index");
        }

    }
}