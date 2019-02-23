using GloriousCore.Models.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GloriousCore.Models.ViewModels
{
    public class OrderVM
    {
        public OrderVM()
        {

        }

        public OrderVM(OrderDBO row)
        {
            Id = row.Id;
            Name = row.Name;
            Number = row.Number;
            Mail = row.Mail;
            City = row.City;
            Addres = row.Addres;
            Post = row.Post;
            Note = row.Note;
            Complete = row.Complete;
        }

        public int Id { get; set; }

        [RegularExpression(@"^[A-zА-яёЁЇїІіЄєҐґ '.-]*$", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string Name { get; set; }

        [RegularExpression(@"[0-9-()+]{3,20}", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string Number { get; set; }

        [RegularExpression(@"^([A-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string Mail { get; set; }

        [RegularExpression(@"^[0-9A-zА-яёЁЇїІіЄєҐґ '.,-/]*$", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string City { get; set; }

        [RegularExpression(@"^[0-9A-zА-яёЁЇїІіЄєҐґ '.,-/]*$", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string Addres { get; set; }

        [RegularExpression(@"^[0-9A-zА-яёЁЇїІіЄєҐґ '.,-/]*$", ErrorMessage = "Недопустимые символы.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string Post { get; set; }

        [RegularExpression(@"^[0-9A-zА-яёЁЇїІіЄєҐґ '.,-/]*$", ErrorMessage = "Недопустимые символы.")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public bool Complete { get; set; }
    }
}
