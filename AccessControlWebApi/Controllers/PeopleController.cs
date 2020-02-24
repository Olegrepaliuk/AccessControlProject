using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        //private AccessCtrlContext db;
        private CRUDRepository repo;
        public PeopleController(AccessCtrlContext context)
        {
            //db = context;
            repo = new CRUDRepository(context);
        }
        // GET api/people
        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetAll()
        {
            List<Person> people = repo.People.ToList();
            return people;
        }

        // GET api/people/5
        [HttpGet("{id}")]
        public ActionResult<Person> Get(int id)
        {
            Person foundPerson = repo.GetPersonById(id);
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
        public ActionResult Post(Person person)
        {
            repo.AddPerson(person);
            //return CreatedAtAction(nameof(Get), new { id = person.Id }, person.Id);
            return StatusCode(201);
        }

        // PUT api/people/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            repo.PutPerson(person);

            return NoContent();
        }

        // DELETE api/people/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = repo.DeletePerson(id);
            if (result == "deleted")
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}