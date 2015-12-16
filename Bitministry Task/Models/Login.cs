using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Bitministry_Task.Models
{
    public class Login
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter the Username")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter the Password")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{7,19}$", ErrorMessage = "7-19 character should be  1caps 1small 1num 1symbol")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Rememberme { get; set; }
    }
}