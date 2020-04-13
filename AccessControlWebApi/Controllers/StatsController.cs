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
    [СustomAuthorization]
    public class StatsController : ControllerBase
    {
        private ControlService controlService;
        public StatsController(ControlService service)
        {
            controlService = service;
        }
        [HttpGet]
        public ActionResult<object> GetAllStats()
        {
            int amountPeopleInside = controlService.CountPeopleInsideNow();
            int amountofPeople = controlService.CountAllPeople();
            return new
            {
                AmountPeopleInside = amountPeopleInside,
                AmountOfPeople = amountofPeople
            };
        }
    }
}