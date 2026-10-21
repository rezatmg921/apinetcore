namespace ApiNetCore.Api.Authentication;

public interface ITokenService
{
    TokenResponse CreateToken(string username);
}
