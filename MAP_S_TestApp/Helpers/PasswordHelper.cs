using System.Security.Cryptography;

namespace MAP_S_TestApp.Helpers;

public class PasswordHelper
{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var verificationPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return verificationPasswordHash.SequenceEqual(passwordHash);
        }
    }

    public static string CreateVerificationToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
}
