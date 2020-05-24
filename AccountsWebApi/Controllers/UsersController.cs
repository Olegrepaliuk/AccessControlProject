using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlModels;
using AccountsWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsWebApi.Controllers
{
    [Route("identity/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private UserService userService;
        public UsersController(UserService service)
        {
            userService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tuple<User, string>>>> GetAllUsersWithDefaultRole()
        {
            var usersWithRoles = await userService.GetAllUsersWithRole();
            return usersWithRoles;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<User>> GetUserById()
        {
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> Create(dynamic userWithRole)
        {
            await userService.CreateUserWithRole(userWithRole);
            return StatusCode(201);
        }

        public ActionResult Update()
        {
            return null;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await userService.DeleteUser(id);
            return NoContent();
        }

    }
}