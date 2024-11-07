using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN61579;

namespace OnlineForms.Models.LogOn
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}