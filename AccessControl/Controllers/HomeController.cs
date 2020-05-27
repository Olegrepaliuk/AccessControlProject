using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AccessControl.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using AccessControlModels;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace AccessControl.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private string baseAddress = "https://localhost:44381/api/stats";
        private static HttpClient client;
        public HomeController(HttpClient cl)
        {
            client = cl;
        }
        
        public async Task<IActionResult> Index()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            int? amtPeopleInside = null;
            int? amtAllPeople = null;
            int? amtSuccessEnters = null;
            int? amtFailedEnters = null;
            if (response.IsSuccessStatusCode)
            {
                var stats = await response.Content.ReadAsAsync<dynamic>();
                amtPeopleInside = stats.amountPeopleInside;
                amtAllPeople = stats.amountOfPeople;
                amtSuccessEnters = stats.successEntersToday;
                amtFailedEnters = stats.failedEntersToday;
            }

            ViewBag.AllPeopleCount = amtAllPeople;
            ViewBag.InBuildNow = amtPeopleInside;
            ViewBag.SuccessEnters = amtSuccessEnters;
            ViewBag.FailedEnters = amtFailedEnters;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Test()
        {
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
