using ApiAuth.Common;
using FluentValidation;

namespace ApiAuth.Auth.Endpoints;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).ValidPassword();
    }
}