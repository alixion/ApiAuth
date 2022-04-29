namespace ApiAuth.Auth.Endpoints;

public record LoginResult(string JwtToken, string RefreshToken);