using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required (ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Required]
        [FutureDateCorrectRange(ErrorMessage = "Date must be later")]
        public DateTime CardValidTil { get; set; }
        [Required]
        public int CardNum { get; set; }

        public Person()
        {

        }
    }
}
