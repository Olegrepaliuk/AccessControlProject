using FoolProof.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [NotEqualTo("OldPassword", ErrorMessage = "New password can not be the same as old")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
