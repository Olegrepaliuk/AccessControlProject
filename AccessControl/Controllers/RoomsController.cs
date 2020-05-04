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

        /*
        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessageWithObj
                (
                    method: HttpMethod.Post,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: currUser.PasswordHash,
                    obj: room
                );
            var response = await client.SendAsync(message);
            return RedirectToAction("Index");
        }
        */

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Room room, List<int> connRooms)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var data = new { Room = room, ConnRooms = connRooms};
            var response = await client.PostAsJsonAsync($"{baseAddress}/createandconnect", data);
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"{baseAddress}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var allOtherRoomsResponse = await client.GetAsync(baseAddress);
                var connectedRoomsResponse = await client.GetAsync($"{baseAddress}/{id}/connectedrooms");
                if (allOtherRoomsResponse.IsSuccessStatusCode && connectedRoomsResponse.IsSuccessStatusCode)
                {
                    var currentRoom = await response.Content.ReadAsAsync<Room>();
                    var otherRooms = await allOtherRoomsResponse.Content.ReadAsAsync<List<Room>>();
                    otherRooms.RemoveAll(room => room.Id == id);
                    var connectedRooms = await connectedRoomsResponse.Content.ReadAsAsync<IEnumerable<Room>>();
                    ViewBag.OtherRooms = otherRooms;
                    if(connectedRooms != null)
                    {
                        var connectedRoomsIds = new HashSet<int>();
                        foreach (var item in connectedRooms)
                        {
                            if(item != null)
                            {
                                connectedRoomsIds.Add(item.Id);
                            }
                            else
                            {
                                connectedRoomsIds.Add(-1);
                            }
                        }
                        ViewBag.ConnectedRoomsIds = connectedRoomsIds;
                    }
                    
                    return View(currentRoom);
                }

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(Room room, List<int> connRooms)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PutAsJsonAsync($"{baseAddress}/{room.Id}", room);
            var roomsResponse = await client.PostAsJsonAsync($"{baseAddress}/{room.Id}/connectedrooms", connRooms);

            return RedirectToAction("Index");
        }
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int roomId)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var reponse = await client.DeleteAsync($"{baseAddress}/{roomId}");
            return RedirectToAction("Index");
        }
    }
}