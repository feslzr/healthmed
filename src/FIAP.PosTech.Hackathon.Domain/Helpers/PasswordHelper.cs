using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace FIAP.PosTech.Hackathon.Domain.Helpers;

[ExcludeFromCodeCoverage]
public static class PasswordHelper
{
    private static readonly string _salt = "secret_salt";
    private static readonly int _iterations = 100000;
    private static readonly int _length = 32;

    public static string HashPassword(string password)
    {
        var salt = Encoding.UTF8.GetBytes(_salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: _iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: _length
        );

        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash.Equals(storedHash);
    }
}