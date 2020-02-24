using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime CardValidTil { get; set; }
        public int CardNum { get; set; }

        public Person()
        {

        }
    }
}
