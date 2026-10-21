namespace ApiNetCore.Api.Authentication;

public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInSeconds);
