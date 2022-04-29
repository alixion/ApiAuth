using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Auth.Endpoints;
using Ardalis.ApiEndpoints;

public class RefreshJwtToken: EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;

    public RefreshJwtToken(AuthDbContext db, ITokensService tokensService)
    {
        _db = db;
        _tokensService = tokensService;
    }

    [HttpPost("refresh-token")]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var token = HttpContext.Request.Headers["Refresh-Token"].FirstOrDefault();
       
        if(token==null)
        {
            return BadRequest("No refresh token was provided");
        }

        var account = await _db.Accounts
            .Include(x => x.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token), cancellationToken);
        
        if (account == null)
            return BadRequest("Invalid token");

        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
        if (refreshToken.IsRevoked)
        {
            RevokeDescendantRefreshTokens(refreshToken, account);
            _db.Update(account);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (!refreshToken.IsActive)
        {
            return BadRequest("Invalid token");
        }

        // generate new refresh token
        var newRefreshToken = _tokensService.GenerateRefreshToken();
        
        // revoke current refresh token
        refreshToken.DateRevoked = DateTimeOffset.UtcNow;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        
        // remove old refresh tokens from account
        _tokensService.RemoveOldRefreshTokens(account);

        _db.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
        
        // generate new jwt
        var jwtToken = _tokensService.GenerateJwtToken(account.Id);
        
        var result = new LoginResult(jwtToken, newRefreshToken.Token);
        return Ok(result);
    }
    
    
    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account account)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if(string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            return;
        
        var childToken = account.RefreshTokens.Single(x => x.Token == refreshToken.ReplacedByToken);
        if (childToken.IsActive)
        {
            childToken.DateRevoked=DateTimeOffset.UtcNow;
        }
        else
            RevokeDescendantRefreshTokens(childToken, account);
    }
}