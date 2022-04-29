namespace ApiAuth.Auth.Model;

public class Account
{
    public Account(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateCreated = DateTimeOffset.UtcNow;
        RefreshTokens = new List<RefreshToken>();
    }

    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTimeOffset DateCreated { get; init; }
    public int AccessFailedCount { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public string? VerificationCode { get; set; }
    public DateTimeOffset? VerificationCodeExpires { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    
    public List<RefreshToken> RefreshTokens { get; set; }
    public bool OwnsRefreshToken(string token) => RefreshTokens?.Find(x => x.Token == token) != null;
}