using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp4.Model
{
    class NContact 
    {
        public string Icon { get; set; }
        
        [Required(ErrorMessage = "Имя обязательно")]
        [MinLength(2, ErrorMessage = "Имя должно содержать минимум 2 символа")]
        [MaxLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Телефон обязателен")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{10,}$", ErrorMessage = "Введите корректный номер телефона")]
        public string Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
        [MaxLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        public string Email { get; set; }

        public string DisplayImage =>
            !string.IsNullOrEmpty(Icon) ? Icon : "user.png";

        public NContact(string name, string phone, string email, string image = null)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Icon = image;
        }
    }
}