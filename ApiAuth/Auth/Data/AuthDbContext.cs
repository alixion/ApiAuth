using ApiAuth.Auth.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Auth.Data;

public class AuthDbContext:DbContext
{
    public AuthDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}