using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlModels;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReadersController : ControllerBase
    {
        private ControlService controlService;

        public ReadersController(ControlService service)
        {
            controlService = service;
        }
        // GET api/people
        [HttpGet]
        public ActionResult<IEnumerable<Reader>> GetAllReaders()
        {
            var readers = controlService.GetAllReaders();
            return readers.ToList();
        }

        // GET api/people/5
        [HttpGet("{id}")]
        public ActionResult<Reader> Get(int id)
        {
            var reader = controlService.GetReaderById(id);
            if (reader == null)
            {
                return NotFound();
            }
            else
            {
                return reader;
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Post([FromBody]Reader reader)
        {
            controlService.CreateReader(reader);
            return StatusCode(201);
        }

  
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Put(int id, [FromBody]Reader reader)
        {
            if (id != reader.Id)
            {
                return BadRequest();
            }
            controlService.UpdateReader(reader);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var result = controlService.DeleteReader(id);
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