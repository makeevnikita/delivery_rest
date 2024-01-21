using System.Text;
using System.Security.Cryptography;


public class HashHelper
{
    private const int SaltSize = 32;

    public string Hash(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltedPassword = new byte[SaltSize + passwordBytes.Length];

        var hash = new SHA256CryptoServiceProvider();

        return Convert.ToBase64String(hash.ComputeHash(saltedPassword));
    }
}