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
    public class RelocationsController : ControllerBase
    {
        private CRUDRepository repo;
        private readonly UserManager<User> _userManager;

        public RelocationsController(AccessCtrlContext context, UserManager<User> userManager)
        {
            repo = new CRUDRepository(context);
            _userManager = userManager;
        }

        public async Task<bool> MoveToOtherLoc(int personId, int toLocId, int realLocId)
        {
            return false;
            /*
            int lastLoggedLocId = repo.FindLastLoggedPersonLoc(personId);
            */
        }
    }
}