using AccessControl.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class AppUsersContext : IdentityDbContext<User>
    {
        public AppUsersContext(DbContextOptions<AppUsersContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
