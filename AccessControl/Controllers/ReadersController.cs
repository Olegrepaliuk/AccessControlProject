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
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"{baseAddress}/{id}");
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Reader reader)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.PostAsJsonAsync(baseAddress, reader);
            return RedirectToAction("Index");
        }

    }
}