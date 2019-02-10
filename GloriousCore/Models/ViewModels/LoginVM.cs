using GloriousCore.Models.Data.Entities;
using GloriousCore.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GloriousCore.Models.ViewModels
{
    public class LoginVM : GoogleReCaptchaModelBase
    {
        public LoginVM()
        {

        }
        public LoginVM(LoginDBO row)
        {
            Id = row.Id;
            Log = row.Log;
            Pass = row.Pass;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(10, ErrorMessage = "Слишком много символов")]
        //[RegularExpression(@"^[0-9A-z]*$", ErrorMessage = "Недопустимые символы.")]
        [Remote("CheckString", "Home", ErrorMessage = "Недопустимые символы.")]
        public string Log { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(10, ErrorMessage = "Слишком много символов")]
        //[RegularExpression(@"^[0-9A-z]*$", ErrorMessage = "Недопустимые символы.")]
        [Remote("CheckString", "Home", ErrorMessage = "Недопустимые символы.")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
    }
}
