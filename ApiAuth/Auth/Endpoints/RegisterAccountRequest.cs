using ApiAuth.Common;
using FluentValidation;

namespace ApiAuth.Auth.Endpoints;

public record RegisterAccountRequest(string Name, string Email, string Password);

public class RegisterAcoountRequestValidator : AbstractValidator<RegisterAccountRequest>
{
    public RegisterAcoountRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).ValidPassword();
    }

    private bool BeValidPassword(string? password)
    {
        if (password == null || password.Length < 8)
            return false;

        return true;
    }
}