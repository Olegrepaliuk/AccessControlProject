using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }
        public string UserId { get; set; }
    }
}
