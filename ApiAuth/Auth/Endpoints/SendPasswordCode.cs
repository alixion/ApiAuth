using ApiAuth.Auth.Data;
using ApiAuth.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Auth.Endpoints;

public class SendPasswordCode : EndpointBaseAsync
    .WithRequest<SendPasswordCodeRequest>
    .WithActionResult
{
    private readonly AuthDbContext _db;
    private readonly ITokensService _tokensService;
    private readonly IMailService _mailService;

    public SendPasswordCode(AuthDbContext db, ITokensService tokensService, IMailService mailService)
    {
        _db = db;
        _tokensService = tokensService;
        _mailService = mailService;
    }

    [HttpPost("forgot-password")]
    public override async Task<ActionResult> HandleAsync(SendPasswordCodeRequest request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var account = await _db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (account == null)
        {
            return Ok();
        }

        account.VerificationCode = _tokensService.GenerateVerificationCode();
        account.VerificationCodeExpires = DateTimeOffset.UtcNow.AddDays(1);
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync(cancellationToken);

        _ = _mailService.SendAsync(account.Email, "Reset your password",
            $"Please use the code below to reset your passord:\n{account.VerificationCode}", cancellationToken);
        return Ok();
    }
}