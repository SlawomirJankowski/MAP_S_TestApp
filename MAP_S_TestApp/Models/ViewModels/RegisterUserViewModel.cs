using System.ComponentModel.DataAnnotations;

namespace MAP_S_TestApp.Models.ViewModels;

public class RegisterUserViewModel
{
    [Required(ErrorMessage = "Pole wymagane")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    [MinLength(8, ErrorMessage = "Hasło musi zawierać co najmniej 8 znaków.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    [Compare("Password", ErrorMessage = "Wprowadzone hasła są różne.")]
    public string ConfirmedPassword { get; set; }
}
