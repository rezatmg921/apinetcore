
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiNetCore.Api.Authentication;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ApiNetCore.UnitTests;

public sealed class JwtTokenServiceTests
{
    private const string TestKey =
        "test-key-for-jwt-unit-tests-at-least-32-bytes";

    private static IConfiguration CreateConfiguration(
        string? key = TestKey)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = key,
            ["Jwt:Issuer"] = "ApiNetCore",
            ["Jwt:Audience"] = "ApiNetCore.Client"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public void CreateToken_ReturnsBearerTokenWithExpectedLifetime()
    {
        var service = new JwtTokenService(CreateConfiguration());

        var result = service.CreateToken("demo");

        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal(1800, result.ExpiresInSeconds);
    }

    [Fact]
    public void CreateToken_IncludesExpectedIssuerAudienceAndUsername()
    {
        var service = new JwtTokenService(CreateConfiguration());

        var result = service.CreateToken("demo");
        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.AccessToken);

        Assert.Equal("ApiNetCore", token.Issuer);
        Assert.Contains("ApiNetCore.Client", token.Audiences);
        Assert.Contains(token.Claims, claim => claim.Value == "demo");
        Assert.Equal(
            "HS256",
            token.Header.Alg);
    }

    [Fact]
    public void CreateToken_ThrowsWhenKeyIsMissing()
    {
        var service = new JwtTokenService(
            CreateConfiguration(key: null));

        Assert.Throws<InvalidOperationException>(
            () => service.CreateToken("demo"));
    }

    [Fact]
    public void CreateToken_ThrowsWhenKeyIsTooShort()
    {
        var service = new JwtTokenService(
            CreateConfiguration(key: "short-key"));

        Assert.Throws<InvalidOperationException>(
            () => service.CreateToken("demo"));
    }
}
