using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            int successEntersToday = controlService.CountSuccessEntersToday();
            int failedEntersToday = controlService.CountFailedEntersToday();
            return new
            {
                AmountPeopleInside = amountPeopleInside,
                AmountOfPeople = amountofPeople,
                SuccessEntersToday = successEntersToday,
                FailedEntersToday = failedEntersToday
            };
        }
    }
}