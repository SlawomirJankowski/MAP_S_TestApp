using System.ComponentModel.DataAnnotations;

namespace MAP_S_TestApp.Models.ViewModels
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Pole wymagane")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole wymagane")]
        public string Password { get; set; }
    }
}
