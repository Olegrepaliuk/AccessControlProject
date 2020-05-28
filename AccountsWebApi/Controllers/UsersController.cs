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
        public async Task<Tuple<User, string>> GetUserById(string id)
        {
            var userWithRole = await userService.GetUserByIdWithRole(id);
            return userWithRole;
        }

        [HttpPost]
        public async Task<ActionResult> Create(dynamic userWithRole)
        {
            await userService.CreateUserWithRole(userWithRole);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody]dynamic obj)
        {
            string newPassword = obj.NewPassword;
            await userService.UpdateUser(newPassword, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await userService.DeleteUser(id);
            return NoContent();
        }

    }
}