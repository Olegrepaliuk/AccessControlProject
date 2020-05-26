using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AccessControlModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Controllers
{
    [Authorize]
    public class ReadersController : Controller
    {
        private string baseAddress = "https://localhost:44381/api/readers";
        private string baseAddressApi = "https://localhost:44381/api";
        private static HttpClient client;
        public ReadersController(HttpClient cl)
        {
            client = cl;
        }

        public async Task<IActionResult> Index()
        {
            List<Reader> allReaders = new List<Reader>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                allReaders = await response.Content.ReadAsAsync<List<Reader>>();
            }           
            return View(allReaders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"{baseAddress}/{id}");
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var allRooms = new List<Room>();
            var allRoomsResponse = await client.GetAsync($"{baseAddressApi}/rooms");
            if (allRoomsResponse.IsSuccessStatusCode)
            {
                allRooms = await allRoomsResponse.Content.ReadAsAsync<List<Room>>();
            }
            ViewBag.AllRooms = allRooms;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Reader reader, int currentRoomId, int nextRoomId)
        {
            if(currentRoomId == 0)
            {
                ModelState.AddModelError("CurrentRoomId", "Please choose current room");
            }
            if(nextRoomId == 0)
            {
                ModelState.AddModelError("NextRoomId", "Please choose next room");
            }
            if (!ModelState.IsValid||(currentRoomId == nextRoomId))
            {
                return RedirectToAction("Create");
            }
            if (currentRoomId != -1) reader.CurrentLocId = currentRoomId;
            if (nextRoomId != -1) reader.NextLocId = nextRoomId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PostAsJsonAsync(baseAddress, reader);
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
                if (allRoomsResponse.IsSuccessStatusCode)
                {
                    var reader = await response.Content.ReadAsAsync<Reader>();
                    var allRooms = await allRoomsResponse.Content.ReadAsAsync<IEnumerable<Room>>();
                    ViewBag.AllRooms = allRooms;
                    return View(reader);
                }

            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(Reader reader, int currentRoomId, int nextRoomId)
        {
            if (!ModelState.IsValid || (currentRoomId == nextRoomId))
            {
                return RedirectToAction("Index");
            }
            reader.NextLocId = null;
            reader.NextLoc = null;
            reader.CurrentLoc = null;
            reader.CurrentLocId = null;
            if (currentRoomId != -1) reader.CurrentLocId = currentRoomId;
            if (nextRoomId != -1) reader.NextLocId = nextRoomId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PutAsJsonAsync($"{baseAddress}/{reader.Id}", reader);
            return RedirectToAction("Index");
        }
    }
}