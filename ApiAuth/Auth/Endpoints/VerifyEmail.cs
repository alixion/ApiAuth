using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using ApiAuth.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ApiAuth.Auth.Endpoints;

[Authorize]
public class VerifyEmail: EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;
    private readonly IMailService _mailService;

    public VerifyEmail(AuthDbContext db, ITokensService tokensService, IMailService mailService)
    {
        _db = db;
        _tokensService = tokensService;
        _mailService = mailService;
    }
    
    [HttpPost("verify-email")]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (HttpContext.Items["Account"] is not Account account)
        {
            return Unauthorized();
        }

        account.VerificationCode = _tokensService.GenerateVerificationCode();
        account.VerificationCodeExpires = DateTimeOffset.UtcNow.AddDays(1);
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync(cancellationToken);

        var message = $"Use the code below to verify your email address.\n{account.VerificationCode}";
        _ = _mailService.SendAsync(account.Email, "Verify your email", message, cancellationToken);
        return Ok();
    }
}