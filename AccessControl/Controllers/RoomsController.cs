using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RoomsController : Controller
    {
        static HttpClient client;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public RoomsController(HttpClient cl, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> rolemanager)
        {
            client = cl;
            _userManager = userManager;
            _signInManager = signInManager;
            _rolemanager = rolemanager;
        }

        private string baseAdress = "https://localhost:44330/api/rooms";

        public async Task<IActionResult> Index()
        {
            List<Room> allRooms = new List<Room>();
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: Request.Cookies["passhash"]//Request.Cookies["passhash"] ?? currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
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
            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessage
                (
                    method: HttpMethod.Get,
                    uri: baseAdress,
                    username: currUser.UserName,
                    password: Request.Cookies["passhash"]//currUser.PasswordHash
                );

            var response = await client.SendAsync(message);
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

            var data = new { Room = room, ConnRooms = connRooms};

            var currUser = await _userManager.GetUserAsync(User);
            var message = RequestBuider.GenerateHttpMessageWithObj
                (
                    method: HttpMethod.Post,
                    uri: baseAdress+"/createandconnect",
                    username: currUser.UserName,
                    password: Request.Cookies["passhash"],//currUser.PasswordHash,
                    obj: data
                );
            var response = await client.SendAsync(message);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Update()
        {
            return null;
        }
    }
}