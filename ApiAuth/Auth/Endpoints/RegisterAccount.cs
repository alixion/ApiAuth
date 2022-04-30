using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ApiAuth.Auth.Endpoints;



public class RegisterAccount: EndpointBaseAsync
    .WithRequest<RegisterAccountRequest>
    .WithActionResult
{
    private readonly AuthDbContext _db;

    public RegisterAccount(AuthDbContext db)
    {
        _db = db;
    }
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Creates a new account",
        Description = "Creates a new account\nEmail must be unique, password should be at least 8 chars",
        OperationId = "Auth.Register"
    )]
    [SwaggerResponse(200,"Account created",typeof(Guid))]
    [SwaggerResponse(400,"Request data is invalid")]
    [SwaggerResponse(500, "Unable to create account")]
    public override async Task<ActionResult> HandleAsync(RegisterAccountRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        if (await _db.Accounts.AnyAsync(x => x.Email == request.Email, cancellationToken: cancellationToken))
        {
            return BadRequest("Email already in use");
        }

        var account = new Account(request.Name, request.Email, BCryptNet.HashPassword(request.Password));

        try
        {
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(account.Id);
        }
        catch (Exception e)
        {
            Log.Error(e,"Could not create account");
            return StatusCode(500);
        }
    }
}