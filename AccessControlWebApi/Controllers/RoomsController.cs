using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlModels;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private ControlService controlService;
        private readonly UserManager<User> _userManager;

        public RoomsController(ControlService service, UserManager<User> userManager)
        {
            controlService = service;
            _userManager = userManager;
        }
        // GET api/rooms
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetAll()
        {
            List<Room> rooms = controlService.GetAllRooms();
            return rooms;
        }

        // GET api/rooms/5
        [HttpGet("{id}")]
        public ActionResult<Room> Get(int id)
        {
            Room foundRoom = controlService.GetRoomById(id);
            if (foundRoom == null)
            {
                return NotFound();
            }
            else
            {
                return foundRoom;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Post([FromBody] Room room)
        {
            controlService.CreateRoom(room);
            return StatusCode(201);
        }

        [HttpPost("createandconnect")]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateAndConnect([FromBody] dynamic obj)
        {
            string roomJson = JsonConvert.SerializeObject(obj.Room);
            Room room = JsonConvert.DeserializeObject<Room>(roomJson);

            string roomsIdsJson = JsonConvert.SerializeObject(obj.ConnRooms);
            List<int> roomsIds = JsonConvert.DeserializeObject<List<int>>(roomsIdsJson);

            controlService.CreateRoom(room);
            controlService.ConnectRoomWithOthers(room.Id, roomsIds);

            return StatusCode(201);
        }

        // PUT api/rooms/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Put(int id, [FromBody]Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }
            controlService.UpdateRoom(room);

            return NoContent();
        }

        // DELETE api/rooms/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            /*
            var result = controlService.DeleteRoom(id);
            if (result == "deleted")
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
            */
            var foundRoom = controlService.GetRoomById(id);
            if (foundRoom == null) return NotFound();
            bool successDeleted = controlService.TryDeleteRoom(id);
            return null;

                
        }

    }
}