using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiNetCore.Api.Authentication;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public TokenResponse CreateToken(string username)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
        if (Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");

        var expires = DateTime.UtcNow.AddMinutes(30);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Name, username)],
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", 1800);
    }
}
