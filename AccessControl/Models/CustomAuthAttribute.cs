using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        filterContext.Result = new RedirectResult("/Error/AccessDenied");
        //    }
        //    else
        //    {
        //        base.HandleUnauthorizedRequest(filterContext);
        //    }
        //}
    }
}
