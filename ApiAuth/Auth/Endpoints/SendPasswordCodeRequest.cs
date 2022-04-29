using FluentValidation;

namespace ApiAuth.Auth.Endpoints;

public record SendPasswordCodeRequest(string Email);

public class SendPasswordCodeRequestValidator : AbstractValidator<SendPasswordCodeRequest>
{
    public SendPasswordCodeRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}