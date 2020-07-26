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

namespace AccessControl.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private static HttpClient client;
        private string baseAddress = "https://localhost:44381/api/rooms";
        public RoomsController(HttpClient cl)
        {
            client = cl;
        }
        public async Task<IActionResult> Index()
        {
            List<Room> allRooms = new List<Room>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                var rooms = await response.Content.ReadAsAsync<IEnumerable<Room>>();
                allRooms = rooms.ToList();
            }
            return View(allRooms);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            List<Room> allRooms = new List<Room>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                var rooms = await response.Content.ReadAsAsync<IEnumerable<Room>>();
                allRooms = rooms.ToList();
            }
            ViewBag.AllRooms = allRooms;
            return View();
        }

  
        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PostAsJsonAsync($"{baseAddress}", room);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"{baseAddress}/{id}");
            if(response.IsSuccessStatusCode)
            {
                var currentRoom = await response.Content.ReadAsAsync<Room>();
                return View(currentRoom);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PutAsJsonAsync($"{baseAddress}/{room.Id}", room);
            return RedirectToAction("Index");
        }
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var reponse = await client.DeleteAsync($"{baseAddress}/{id}");
            return RedirectToAction("Index");
        }
    }
}