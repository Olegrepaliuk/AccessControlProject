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
        public string Position { get; set; }
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone format: +380XXXXXXXXX")]
        public string Phone { get; set; }
        [RegularExpression(@"^[A-Za-z]+[\.A-Za-z0-9_-]*@[A-Za-z]+\.[A-Za-z]+", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required]
        [FutureDateCorrectRange(ErrorMessage = "Date must be later")]
        public DateTime CardValidTil { get; set; }
        [Required]
        public string CardKey { get; set; }

        public Person()
        {

        }
    }
}
