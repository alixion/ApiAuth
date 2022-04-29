using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Auth.Endpoints;

[Authorize]
public class RevokeRefreshToken: EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;

    public RevokeRefreshToken(AuthDbContext db, ITokensService tokensService)
    {
        _db = db;
        _tokensService = tokensService;
    }

    [HttpPost("revoke-refresh-token")]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // accept refresh token from request body and from http header
        var token = HttpContext.Request.Headers["Refresh-Token"].FirstOrDefault();
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Refresh token is required");
        }

        if (HttpContext.Items["Account"] is not Account account || !account.OwnsRefreshToken(token))
        {
            return Unauthorized();
        }

        var refreshToken = await _db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
        if (refreshToken == null || !refreshToken.IsActive)
        {
            return BadRequest("Refresh token not found or inactive");
        }
        
        refreshToken.DateRevoked = DateTimeOffset.UtcNow;
        _db.Update(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}