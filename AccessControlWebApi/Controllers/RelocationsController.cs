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

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RelocationsController : ControllerBase
    {
        private ControlService controlService;
        public RelocationsController(ControlService service)
        {
            controlService = service;
        }

        [HttpPost("move")]
        [Authorize(Roles = "Admin")]
        public ActionResult<bool> MoveToOtherLoc(int personId, int? toLocId, int? realLocId)
        {
            int? lastLoggedLocId = controlService.FindLastLoggedPersonLocId(personId);
            if ((lastLoggedLocId != realLocId) & lastLoggedLocId != -1)
            {
                //DoSmth
            }
            return controlService.TryMoveToOtherLoc(personId, realLocId, toLocId);
            
        }

        [HttpGet]
        public ActionResult <IEnumerable<Relocation>> GetRelocationsHistory()
        {
            return controlService.GetAllRelocations().ToList();
        }

    }
}