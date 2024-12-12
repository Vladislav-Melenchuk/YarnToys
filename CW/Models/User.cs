using System.ComponentModel.DataAnnotations;

namespace CW.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(50, ErrorMessage = "Имя пользователя не может превышать 50 символов.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Введите корректный номер телефона.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [MinLength(8, ErrorMessage = "Пароль должен быть не менее 8 символов.")]
        public string Password { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; } 

        public bool IsAdmin { get; set; }
    }
}
