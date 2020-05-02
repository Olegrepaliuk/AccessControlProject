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
        public async Task<ActionResult<IEnumerable<Tuple<User, string>>>> GetAllUsersWithDefaultRole()
        {
            var usersWithRoles = await userService.GetAllUsersWithRole();
            return usersWithRoles;
        }
        [HttpPost]
        public ActionResult Create()
        {
            return null;
        }

        public ActionResult Delete()
        {
            return null;
        }

    }
}