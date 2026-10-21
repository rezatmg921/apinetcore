using Xunit;
using ApiNetCore.Api.Authentication;
using Microsoft.Extensions.Configuration;

namespace ApiNetCore.UnitTests;

public sealed class DemoCredentialValidatorTests
{
    [Fact]
    public void IsValid_ReturnsTrueForConfiguredCredentials()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Auth:DemoUsername"] = "demo",
            ["Auth:DemoPassword"] = "a-long-test-password"
        }).Build();
        var validator = new DemoCredentialValidator(configuration);

        Assert.True(validator.IsValid("demo", "a-long-test-password"));
    }

    [Fact]
    public void IsValid_ReturnsFalseForWrongPassword()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Auth:DemoUsername"] = "demo",
            ["Auth:DemoPassword"] = "a-long-test-password"
        }).Build();
        var validator = new DemoCredentialValidator(configuration);

        Assert.False(validator.IsValid("demo", "wrong-password"));
    }

    [Fact]
    public void IsValid_ReturnsFalseWhenCredentialsAreNotConfigured()
    {
        var validator = new DemoCredentialValidator(new ConfigurationBuilder().Build());

        Assert.False(validator.IsValid("demo", "password"));
    }
}
