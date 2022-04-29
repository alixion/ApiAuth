namespace ApiAuth.Auth.Model;

public class RefreshToken
{
    public RefreshToken(string token, DateTimeOffset dateExpires)
    {
        Id = Guid.NewGuid();
        Token = token;
        DateCreated = DateTimeOffset.UtcNow;
        DateExpires = dateExpires;
    }
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string? ReplacedByToken { get; set; }

    public DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset DateExpires { get; set; }
    public DateTimeOffset? DateRevoked { get; set; }

    public Account Account { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow > DateExpires.UtcDateTime;
    public bool IsRevoked => DateRevoked != null;
    public bool IsActive => !IsExpired && !IsRevoked;
}