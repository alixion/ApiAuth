using ApiAuth.Auth.Data;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ApiAuth.Auth.Endpoints;

public class ResetPassword: EndpointBaseAsync
    .WithRequest<ResetPasswordRequest>
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;

    public ResetPassword(AuthDbContext db, ITokensService tokensService)
    {
        _db = db;
        _tokensService = tokensService;
    }

    [HttpPost("reset-password")]
    public override async Task<ActionResult> HandleAsync(ResetPasswordRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var account = await _db.Accounts.SingleOrDefaultAsync(x => x.VerificationCode == request.VerificationCode, cancellationToken);
        
        if (account?.VerificationCodeExpires == null || account.VerificationCodeExpires.Value < DateTimeOffset.UtcNow)
            return BadRequest("Password reset failed");


        account.PasswordHash = BCryptNet.HashPassword(request.NewPassword);
        account.VerificationCode = null;
        account.VerificationCodeExpires = null;
        
        _db.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}