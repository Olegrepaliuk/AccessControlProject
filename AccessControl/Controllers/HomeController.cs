using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AccessControl.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace AccessControl.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController(HttpClient cl)
        {
            //client.BaseAddress = new Uri("https://localhost:44330/");
            //client.BaseAddress = new Uri("http://localhost:6666/");
            client = cl;
        }
        static HttpClient client;
        public IActionResult Index()
        {
            return View();
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

        public async Task <IActionResult> Test()
        {
            //var response = await client.GetAsync($"api/values/5");
            var response = await client.GetAsync($"animals");
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
            }

            var response2 = await client.GetAsync($"api/people");
            if (response2.IsSuccessStatusCode)
            {
                var s2 = await response2.Content.ReadAsAsync<IEnumerable<Person>>();
            }


            return View("Index");
        }

    }
}
