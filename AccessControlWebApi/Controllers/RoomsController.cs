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
    public class RoomsController : ControllerBase
    {
        private CRUDRepository repo;
        private readonly UserManager<User> _userManager;

        public RoomsController(AccessCtrlContext context, UserManager<User> userManager)
        {
            repo = new CRUDRepository(context);
            _userManager = userManager;
        }
        // GET api/rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAll()
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            List<Room> rooms = repo.Rooms.ToList();
            return rooms;
        }

        // GET api/rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var user = await CheckAuthorization();
            if (user == null) return Unauthorized();
            Room foundRoom = repo.GetRoomById(id);
            if (foundRoom == null)
            {
                return NotFound();
            }
            else
            {
                return foundRoom;
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