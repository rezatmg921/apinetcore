using System.Security.Cryptography;
using System.Text;

namespace ApiNetCore.Api.Authentication;

/// <summary>Development/bootstrap-only credential check. Replace with a real identity provider before production.</summary>
public sealed class DemoCredentialValidator(IConfiguration configuration)
{
    public bool IsValid(string username, string password)
    {
        var expectedUsername = configuration["Auth:DemoUsername"];
        var expectedPassword = configuration["Auth:DemoPassword"];
        if (string.IsNullOrEmpty(expectedUsername) || string.IsNullOrEmpty(expectedPassword)) return false;

        return FixedEquals(username, expectedUsername) && FixedEquals(password, expectedPassword);
    }

    private static bool FixedEquals(string actual, string expected) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(actual), Encoding.UTF8.GetBytes(expected));
}
