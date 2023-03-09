using System.ComponentModel.DataAnnotations;

namespace MAP_S_TestApp.Models.Domains;

public class ApplicationUser
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Pole wymagane")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
    public string Email { get; set; }

    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActivated { get; set; }
    public string VerificationToken { get; set; }

}
