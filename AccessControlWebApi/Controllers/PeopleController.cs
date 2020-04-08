﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using AccessControlWebApi.Models;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlWebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [СustomAuthorization]
    public class PeopleController : ControllerBase
    {
        private ControlService controlService;
        private readonly UserManager<User> _userManager;

        public PeopleController(ControlService service, UserManager<User> userManager)
        {
            //db = context;
            controlService = service;
            _userManager = userManager;
        }
        // GET api/people
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            List<Person> people = controlService.GetAllPeople();
            return people;
        }

        // GET api/people/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> Get(int id)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            Person foundPerson = controlService.GetPersonById(id);
            if(foundPerson == null)
            {
                return NotFound();
            }
            else
            {
                return foundPerson;
            }
        }

        // POST api/people
        [HttpPost]
        [СustomAuthorization(OnlyAdmin = true)]
        public async Task<ActionResult> Post([FromBody]Person person)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            //var hasRight = await CheckRights(user);
            //if (!hasRight) return Forbid();
            controlService.CreatePerson(person);
            //return CreatedAtAction(nameof(Get), new { id = person.Id }, person.Id);
            return StatusCode(201);
        }

        // PUT api/people/5
        [HttpPut("{id}")]
        [СustomAuthorization(OnlyAdmin = true)]
        public async Task<ActionResult> Put(int id, [FromBody]Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            //var hasRight = await CheckRights(user);
            //if (!hasRight) return Forbid();

            controlService.UpdatePerson(person);

            return NoContent();
        }

        // DELETE api/people/5
        [HttpDelete("{id}")]
        [СustomAuthorization(OnlyAdmin = true)]
        public async Task<ActionResult> Delete(int id)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            //var hasRight = await CheckRights(user);
            //if (!hasRight) return Forbid();

            var result = controlService.DeletePerson(id);
            if (result == "deleted")
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsOfPersonAccess(int id)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();

            var person = controlService.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }
            return controlService.GetRoomsOfPersonAccess(person.Id).ToList();
        }

        [HttpPost("{id}/rooms")]
        [СustomAuthorization(OnlyAdmin = true)]
        public async Task<ActionResult> UpdatePersonAccess(int id, [FromBody]IEnumerable<int> roomsId)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            //var hasRight = await CheckRights(user);
            //if (!hasRight) return Forbid();

            var person = controlService.GetPersonById(id);
            if(person == null)
            {
                return NotFound();
            }
            controlService.UpdatePersonAccess(id, roomsId);
            return NoContent();
        }

        /*
        private async Task<User> CheckAuthorization()
        {
            //var UserManager = HttpContext.GetOwinContext().GetUserManager<User>();
            var req = Request;
            if(!req.Headers.ContainsKey("username"))
            {
                return null;
            }
            if(req.Headers.ContainsKey("password"))
            {
                var retrievedUser = await UsersInfo.FindUser(req.Headers["username"], req.Headers["password"], _userManager, false);
                return retrievedUser;
            }
            else
            {
                if (req.Headers.ContainsKey("passhash"))
                {
                    var retrievedUser = await UsersInfo.FindUser(req.Headers["username"], req.Headers["passhash"], _userManager);
                    return retrievedUser;
                }
                return null;
            }

        }

        private async Task<bool> CheckRights(User user)
        {
            var hasRight = await UsersInfo.CheckAdminRights(user, _userManager);
            return hasRight;
        }
        */
    }
}