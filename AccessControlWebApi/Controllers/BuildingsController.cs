using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlModels;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private ControlService controlService;
        private readonly UserManager<User> _userManager;

        public BuildingsController(ControlService service, UserManager<User> userManager)
        {
            //db = context;
            controlService = service;
            _userManager = userManager;
        }

        // GET api/buildings
        [HttpGet]
        public ActionResult<IEnumerable<Building>> GetAll()
        {
            List<Building> buildings = controlService.GetAllBuildings();
            return buildings;
        }

        // GET api/buildings/5
        [HttpGet("{id}")]
        public ActionResult<Building> Get(int id)
        {
            var foundBuilding = controlService.GetBuildingById(id);
            if (foundBuilding == null)
            {
                return NotFound();
            }
            else
            {
                return foundBuilding;
            }
        }

        [HttpGet("{id}/rooms")]
        public ActionResult<IEnumerable<Room>> GetRooms(int id)
        {
            var foundBuilding = controlService.GetBuildingById(id);
            if (foundBuilding == null)
            {
                return NotFound();
            }
            else
            {
                var rooms = controlService.GetRoomsOfBuilding(id);
                return rooms;
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody]Building building)
        {
            controlService.CreateBuilding(building);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]Building building)
        {
            if (id != building.Id)
            {
                return BadRequest();
            }
            controlService.UpdateBuilding(building);

            return NoContent();
        }

        // DELETE api/rooms/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = controlService.DeleteRoom(id);
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