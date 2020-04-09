using AccessControlModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class СustomAuthorization : ActionFilterAttribute, IAuthorizationFilter
    {
        public bool OnlyAdmin { get; set; }
        public СustomAuthorization()
        {

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userService = context.HttpContext.RequestServices.GetService<UserService>();
            var req = context.HttpContext.Request;
            if (!req.Headers.ContainsKey("username"))
            {
                Unauthorize(context);
                return;
            }

            bool? passhashed = null;
            string password = "";
            if (req.Headers.ContainsKey("password"))
            {
                passhashed = false;
                password = req.Headers["password"];
            }
            if(req.Headers.ContainsKey("passhash"))
            {
                passhashed = true;
                password = req.Headers["passhash"];
            }
            if(passhashed == null)
            {
                Unauthorize(context);
                return;
            }
            var retrievedUser = AsyncHelper.RunSync<User>(() => userService.FindUserAsync(req.Headers["username"], password, passhashed.Value));
            if (retrievedUser != null)
            {
                if (OnlyAdmin)
                {
                    bool access = AsyncHelper.RunSync<bool>(() => userService.CheckAdminRights(retrievedUser));
                    if (!access)
                    {
                        Unauthorize(context);
                    }
                }
                return;
            }

            Unauthorize(context);
        }
        private void Unauthorize(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
