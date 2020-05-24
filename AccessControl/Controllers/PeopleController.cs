using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {        
        private string baseAddressApi = "https://localhost:44381/api";
        private string baseAddress = "https://localhost:44381/api/people";
        private static HttpClient client;
        public PeopleController(HttpClient cl)
        {
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                var people = await response.Content.ReadAsAsync<IEnumerable<Person>>();
                allPeople = people.ToList(); 
            }
            var peopleInsideIds = new HashSet<int>();
            var peopleInsideResponse = await client.GetAsync($"{baseAddress}/inside/idslist");
            if(peopleInsideResponse.IsSuccessStatusCode)
            {
                string json = await peopleInsideResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<HashSet<int>>(json);
                peopleInsideIds = result;
            }
            ViewBag.PeopleInsideIds = peopleInsideIds;
            return View(allPeople);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"{baseAddress}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            if(!ModelState.IsValid)
            {
                return View(person);
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PostAsJsonAsync(baseAddress, person);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"{baseAddress}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var allRoomsResponse = await client.GetAsync($"{baseAddressApi}/rooms");
                var personRoomsResponse = await client.GetAsync($"{baseAddress}/{id}/rooms");
                if(allRoomsResponse.IsSuccessStatusCode&&personRoomsResponse.IsSuccessStatusCode)
                {
                    var person = await response.Content.ReadAsAsync<Person>();
                    var personRooms = await personRoomsResponse.Content.ReadAsAsync<IEnumerable<Room>>();
                    var allRooms = await allRoomsResponse.Content.ReadAsAsync<IEnumerable<Room>>();
                    ViewBag.AllRooms = allRooms;
                    ViewBag.PersonRooms = personRooms;
                    return View(person);
                }

            }
            return RedirectToAction("Index");
        }

        /*
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
        */

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(Person person, List<int> rooms)
        {
            if (!ModelState.IsValid)
            {
                return View(person);
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PutAsJsonAsync($"{baseAddress}/{person.Id}",person);
            var roomsResponse = await client.PostAsJsonAsync($"{baseAddress}/{person.Id}/rooms", rooms);

            return RedirectToAction("Index");
        }

    }
}