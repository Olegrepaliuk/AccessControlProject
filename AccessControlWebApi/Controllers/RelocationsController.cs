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


        [HttpPost("move/{readerId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<object> MoveToOtherLoc(int readerId, [FromBody]dynamic obj)
        {
            string cardKey = obj.cardKey;
            var allowed = controlService.TryMoveToOtherLoc(cardKey, readerId);
            return new { Allowed = allowed };
        }

        [HttpGet]
        public ActionResult <IEnumerable<Relocation>> GetRelocationsHistory()
        {
            return controlService.GetAllRelocations().ToList();
        }
        [HttpGet("generate")]
        public string GenerateData()
        {
            controlService.GenerateData();
            return "generated";
        }
    }
}