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
        private CRUDRepository repo;
        private readonly UserManager<User> _userManager;

        public BuildingsController(AccessCtrlContext context, UserManager<User> userManager)
        {
            //db = context;
            repo = new CRUDRepository(context);
            _userManager = userManager;
        }

        // GET api/buildings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetAll()
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            List<Building> buildings = repo.Buildings.ToList();
            return buildings;
        }

        // GET api/buildings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Building>> Get(int id)
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            var foundBuilding = repo.GetBuildingById(id);
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
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms(int id)
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            var foundBuilding = repo.GetBuildingById(id);
            if (foundBuilding == null)
            {
                return NotFound();
            }
            else
            {
                var rooms = repo.GetRoomsOfBuilding(id).ToList();
                return rooms;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Building building)
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            var hasRight = await CheckRights(user);
            if (!hasRight) return Forbid();
            repo.AddBuilding(building);
            //return CreatedAtAction(nameof(Get), new { id = person.Id }, person.Id);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]Building building)
        {
            if (id != building.Id)
            {
                return BadRequest();
            }

            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            var hasRight = await CheckRights(user);
            if (!hasRight) return Forbid();

            repo.PutBuilding(building);

            return NoContent();
        }

        // DELETE api/rooms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            var hasRight = await CheckRights(user);
            if (!hasRight) return Forbid();
            var result = repo.DeleteRoom(id);
            if (result == "deleted")
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        private async Task<User> CheckAuthorization()
        {
            var req = Request;
            if (!req.Headers.ContainsKey("username"))
            {
                return null;
            }
            if (req.Headers.ContainsKey("password"))
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
    }
}