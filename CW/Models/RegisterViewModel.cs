using System.ComponentModel.DataAnnotations;

namespace CW.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(50, ErrorMessage = "Имя пользователя должно быть не более 50 символов.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен.")]
        [Phone(ErrorMessage = "Введите действительный номер телефона.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} символов.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
