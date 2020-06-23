using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCalcUI.Models
{
    public class DisplayPersonModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "First Name is too long.")]
        [MinLength(1, ErrorMessage = "First Name is too short.")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Last Name is too long.")]
        [MinLength(1, ErrorMessage = "Last Name is too short.")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
