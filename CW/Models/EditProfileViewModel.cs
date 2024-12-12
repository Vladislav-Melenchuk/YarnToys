using System.ComponentModel.DataAnnotations;

namespace CW.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен.")]
        public string PhoneNumber { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Для подтверждения внесённых изменений введите ваш текущий пароль.")]
        public string CurrentPassword { get; set; }
    }
}
