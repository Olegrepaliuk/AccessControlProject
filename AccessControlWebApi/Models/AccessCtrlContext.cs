using AccessControl.Models;
using AccessControlModels;
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
        //public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PersonRoom> PersonRoom { get; set; }
        public DbSet<Relocation> Relocations { get; set; }
        public DbSet<Door> Doors { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public AccessCtrlContext(DbContextOptions<AccessCtrlContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonRoom>().HasKey(pr => new { pr.PersonId, pr.RoomId });
        }
        
        public AccessCtrlContext():base()
        {
            
        }
    }
}
