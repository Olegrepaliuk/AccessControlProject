
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
