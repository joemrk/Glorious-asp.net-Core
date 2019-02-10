using System;
using System.ComponentModel.DataAnnotations;
using GloriousCore.Validation;
using Newtonsoft.Json;

namespace GloriousCore.Models
{
    public class Comment : GoogleReCaptchaModelBase
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String Content { get; set; }
    }
}
