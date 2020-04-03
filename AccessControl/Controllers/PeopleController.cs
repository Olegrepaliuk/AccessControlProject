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
    [Authorize]
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

        private string baseAdressApi = "https://localhost:44330/api";
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
                var allRoomsMessage = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdressApi + "/rooms",
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );
                var allRoomsResponse = await client.SendAsync(allRoomsMessage);

                var pesonRoomsMessage = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress + "/" + id + "/rooms",
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );
                var personRoomsResponse = await client.SendAsync(pesonRoomsMessage);
                
                if(allRoomsResponse.IsSuccessStatusCode&&personRoomsResponse.IsSuccessStatusCode)
                {
                    var person = await response.Content.ReadAsAsync<Person>();
                    var personRooms = await personRoomsResponse.Content.ReadAsAsync<IEnumerable<Room>>();
                    //personRooms.Where(pr => pr.Id == id).
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

            var roomsMessage = RequestBuider.GenerateHttpMessageWithObj
            (
                method: HttpMethod.Post,
                uri: baseAdress + "/" + person.Id+"/rooms",
                username: currUser.UserName,
                password: currUser.PasswordHash,
                obj: rooms
            );

            var roomsResponse = await client.SendAsync(roomsMessage);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Test()
        {
            Room room = new Room();
            //var response = await Task.Run(()=>client.GetAsync($"api/people"));
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: "https://localhost:44330/api/rooms/1",
                    username: currUser.UserName,
                    password: currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                room = await response.Content.ReadAsAsync<Room>();
            }

            return View(room);
        }

    }
}