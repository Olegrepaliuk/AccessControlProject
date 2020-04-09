using System;
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
        [СustomAuthorization(OnlyAdmin = true)]
        public ActionResult Post([FromBody]Person person)
        {
            controlService.CreatePerson(person);
            return StatusCode(201);
        }

        // PUT api/people/5
        [HttpPut("{id}")]
        [СustomAuthorization(OnlyAdmin = true)]
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
        [СustomAuthorization(OnlyAdmin = true)]
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
        [СustomAuthorization(OnlyAdmin = true)]
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

    }
}