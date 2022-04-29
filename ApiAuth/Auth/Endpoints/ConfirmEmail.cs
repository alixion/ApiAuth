using ApiAuth.Auth.Data;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Auth.Endpoints;

public class ConfirmEmail: EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
{
    private readonly AuthDbContext _db;

    public ConfirmEmail(AuthDbContext db)
    {
        _db = db;
    }

    [HttpGet("confirm-email")]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var securityStamp = Request.Query["token"].FirstOrDefault();
        if (securityStamp == null)
            return BadRequest("No verification code was provided");

        var account = await _db.Accounts.SingleOrDefaultAsync(x =>x.VerificationCode == securityStamp, cancellationToken);


        if (account?.VerificationCodeExpires == null || account.VerificationCodeExpires.Value < DateTimeOffset.UtcNow)
            return BadRequest("Verification failed");
        
        account.EmailConfirmed = true;
        account.VerificationCode = null;
        account.VerificationCodeExpires = null;
        
        _db.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}