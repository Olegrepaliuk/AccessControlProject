using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class AccessCtrlContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public AccessCtrlContext(DbContextOptions<AccessCtrlContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public AccessCtrlContext():base()
        {

        }
    }
}
