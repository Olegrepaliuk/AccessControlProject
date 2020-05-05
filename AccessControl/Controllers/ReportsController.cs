using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private static HttpClient client;
        private string baseAddress = "https://localhost:44381/api/reports";
        public ReportsController(HttpClient cl)
        {
            client = cl;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Dates")]
        public async Task<IActionResult> GetReportByDates()
        {
            var allGroupedRelocations= new List<IGrouping<DateTime, Relocation>>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Relocation>>(json);
                allGroupedRelocations = result.GroupBy(rel => rel.DateAndTime.Date).ToList();
            }

            return View(allGroupedRelocations);
        }

        [HttpGet]
        public async Task<IActionResult> People()
        {
            var allGroupedRelocations = new List<IGrouping<int, Relocation>>();
            /*
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Relocation>>(json);
                allGroupedRelocations = result.GroupBy(rel => rel.PersonId).ToList();
            }
            */
            return View(allGroupedRelocations);
        }

        [HttpGet("Rooms")]
        public async Task<IActionResult> GetReportByRooms()
        {
            var allGroupedRelocations = new List<IGrouping<Person, Relocation>>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Relocation>>(json);
                //allGroupedRelocations = result.GroupBy(rel => rel.ToLocId).ToList();
            }

            return View(allGroupedRelocations);
        }

    }
}