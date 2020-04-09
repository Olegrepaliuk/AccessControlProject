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
        public ActionResult<bool> MoveToOtherLoc(int personId, int? toLocId, int? realLocId)
        {
            int? lastLoggedLocId = controlService.FindLastLoggedPersonLocId(personId);
            if ((lastLoggedLocId != realLocId) & lastLoggedLocId != -1)
            {
                //DoSmth
            }
            return controlService.TryMoveToOtherLoc(personId, realLocId, toLocId);
            
        }

    }
}