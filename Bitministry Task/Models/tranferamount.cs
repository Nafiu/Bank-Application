using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Bitministry_Task.Models
{
    public class tranferamount
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter the Username")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter the Field")]
        public string To { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter the Currency")]
        [DataType(DataType.Currency)]
        public int Amount { get; set; }
    }
}