using ApiAuth.Auth.Data;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ApiAuth.Auth.Endpoints;

public class Login: EndpointBaseAsync
    .WithRequest<LoginRequest>
    .WithActionResult<LoginResult>
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;

    public Login(AuthDbContext db, ITokensService tokensService)
    {
        _db = db;
        _tokensService = tokensService;
    }
    
    [HttpPost("login")]
    public override async Task<ActionResult<LoginResult>> HandleAsync(LoginRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var account = await _db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (account == null)
            return BadRequest("Login failed");
        if (account.LockoutEnd != null && account.LockoutEnd > DateTimeOffset.UtcNow)
            return BadRequest("Account is locked");

        if (!BCryptNet.Verify(request.Password, account.PasswordHash))
        {
            account.AccessFailedCount++;
            await _db.SaveChangesAsync(cancellationToken);
            return BadRequest("Login failed");
        }
        
        var jwtToken = _tokensService.GenerateJwtToken(account.Id);
        var refreshToken = _tokensService.GenerateRefreshToken();
        account.RefreshTokens.Add(refreshToken);
        _tokensService.RemoveOldRefreshTokens(account);

        _db.Update(account);
        await _db.SaveChangesAsync(cancellationToken);

        var result = new LoginResult(jwtToken, refreshToken.Token);
        return Ok(result);
    }

    
    
    
}