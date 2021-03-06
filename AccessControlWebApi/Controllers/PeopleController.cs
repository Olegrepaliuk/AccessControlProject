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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PeopleController : ControllerBase
    {
        private ControlService controlService;

        public PeopleController(ControlService service)
        {
            controlService = service;
        }
        // GET api/people
        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetAll()
        {
            List<Person> people = controlService.GetAllPeople();
            return people;
        }

        // GET api/people/5
        [HttpGet("{id}")]
        public ActionResult<Person> Get(int id)
        {
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
        [Authorize(Roles = "Admin")]
        public ActionResult Post([FromBody]Person person)
        {
            controlService.CreatePerson(person);
            return StatusCode(201);
        }

        // PUT api/people/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Put(int id, [FromBody]Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }
            controlService.UpdatePerson(person);
            return NoContent();
        }

        // DELETE api/people/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
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
        public ActionResult<IEnumerable<Room>> GetRoomsOfPersonAccess(int id)
        {
            var person = controlService.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }
            return controlService.GetRoomsOfPersonAccess(person.Id).ToList();
        }

        [HttpPost("{id}/rooms")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdatePersonAccess(int id, [FromBody]IEnumerable<int> roomsId)
        {
            var person = controlService.GetPersonById(id);
            if(person == null)
            {
                return NotFound();
            }
            controlService.UpdatePersonAccess(id, roomsId);
            return NoContent();
        }

        [HttpGet("inside")]
        public ActionResult<IEnumerable<Person>> GetPeopleInsideNow()
        {
            return controlService.GetPeopleInsideNow();
        }

        [HttpGet("inside/idslist")]
        public ActionResult<IEnumerable<int>> GetIdsPeopleInsideNow()
        {
            return controlService.GetPeopleIdsInsideNow();
        }
    }
}