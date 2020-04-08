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
    [СustomAuthorization]
    public class RelocationsController : ControllerBase
    {
        private ControlService controlService;
        private readonly UserManager<User> _userManager;


        public RelocationsController(ControlService service, UserManager<User> userManager)
        {
            controlService = service;
            _userManager = userManager;
        }

        [СustomAuthorization(OnlyAdmin = true)]
        public async Task<ActionResult<bool>> MoveToOtherLoc(int personId, int? toLocId, int? realLocId)
        {
            //var user = await CheckAuthorization();
            //if (user == null) return Unauthorized();
            //var hasRight = await CheckRights(user);
            //if (!hasRight) return Forbid();
            int? lastLoggedLocId = controlService.FindLastLoggedPersonLocId(personId);
            if ((lastLoggedLocId != realLocId) & lastLoggedLocId != -1)
            {
                //DoSmth
            }
            return controlService.TryMoveToOtherLoc(personId, realLocId, toLocId);
            
        }

        /*
        private async Task<User> CheckAuthorization()
        {
            //var UserManager = HttpContext.GetOwinContext().GetUserManager<User>();
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
        */
    }
}