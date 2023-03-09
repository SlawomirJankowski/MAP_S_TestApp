namespace MAP_S_TestApp.Models.Domains;

public class ApplicationUser
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActivated { get; set; }
    public string VerificationToken { get; set; }

}
