using ApiAuth.Common;
using FluentValidation;

namespace ApiAuth.Auth.Endpoints;

public record ResetPasswordRequest(string VerificationCode, string NewPassword);

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.VerificationCode).NotEmpty();
        RuleFor(x => x.NewPassword).ValidPassword();
    }
}