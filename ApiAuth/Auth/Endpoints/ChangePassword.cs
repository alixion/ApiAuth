using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ApiAuth.Auth.Endpoints;

[Authorize]
public class ChangePassword: EndpointBaseAsync
    .WithRequest<ChangePasswordRequest>
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;

    public ChangePassword(AuthDbContext db, ITokensService tokensService)
    {
        _db = db;
        _tokensService = tokensService;
    }

    [HttpPost("change-password")]
    public override async Task<ActionResult> HandleAsync(ChangePasswordRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        if (HttpContext.Items["Account"] is not Account account)
        {
            return Unauthorized();
        }

        var currentPasswordHash = BCryptNet.HashPassword(request.CurrentPassword);

        if (currentPasswordHash != account.PasswordHash)
        {
            return BadRequest("Password change failed");
        }

        account.PasswordHash = BCryptNet.HashPassword(request.NewPassword);
        
        _db.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}