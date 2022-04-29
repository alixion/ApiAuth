using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApiAuth.Auth.Data;
using ApiAuth.Auth.Model;
using ApiAuth.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuth.Auth;

public interface ITokensService
{
    string GenerateJwtToken(Guid accountId);
    Guid? ValidateJwtToken(string? token);
    RefreshToken GenerateRefreshToken();
    void RemoveOldRefreshTokens(Account account);

    string GenerateVerificationCode();
}

public class TokensService: ITokensService
{
    private readonly AuthDbContext _db;
    private readonly AppSettings _appSettings;

    public TokensService(AuthDbContext db, IOptions<AppSettings> appSettingsOptions)
    {
        _db = db;
        _appSettings = appSettingsOptions.Value;
    }
    
    
    public string GenerateJwtToken(Guid accountId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", accountId.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public Guid? ValidateJwtToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // return account id from JWT token if validation successful
            return accountId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    public RefreshToken GenerateRefreshToken()
    {
        while (true)
        {
            var refreshToken = new RefreshToken(Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
                DateTimeOffset.UtcNow.AddDays(_appSettings.RefreshTokenDaysToLive));

            // ensure token is unique by checking against db
            var tokenIsUnique = !_db.Accounts.Any(a => a.RefreshTokens.Any(t => t.Token == refreshToken.Token));

            if (tokenIsUnique) 
                return refreshToken;
        }
    }

    public string GenerateVerificationCode()
    {
        while (true)
        {
            var verificationCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var stampIsUnique = !_db.Accounts.Any(a => a.VerificationCode == verificationCode);

            if (stampIsUnique)
                return verificationCode;
        }
    }

    public void RemoveOldRefreshTokens(Account account)
    {
        account.RefreshTokens.RemoveAll(x => 
            !x.IsActive && 
            x.DateCreated.AddDays(_appSettings.RefreshTokenDaysToLive) <= DateTimeOffset.UtcNow);
    }
}