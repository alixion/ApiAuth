using ApiAuth.Auth.Data;

namespace ApiAuth.Auth;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AuthDbContext db, ITokensService tokensService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        Guid? accountId = tokensService.ValidateJwtToken(token);

        if (accountId != null)
        {
            context.Items["Account"] = await db.Accounts.FindAsync(accountId.Value);
        }

        await _next(context);
    }
}