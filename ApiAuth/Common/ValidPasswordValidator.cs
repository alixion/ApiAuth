using FluentValidation;
using FluentValidation.Validators;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuth.Common;

public class ValidPasswordValidator<T,TProperty> : PropertyValidator<T,TProperty> {

    public override string Name => "ValidPasswordValidator";

    public override bool IsValid(ValidationContext<T> context, TProperty value) {
        if (value is not string stringValue)
        {
            return false;
        }
        
        if (string.IsNullOrEmpty(stringValue) || stringValue.Length < 8)
            return false;

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "'{PropertyName}' must not be a valid a password.";
}

public static class MyValidatorExtensions {
    public static IRuleBuilderOptions<T, TProperty> ValidPassword<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new ValidPasswordValidator<T, TProperty>());
    }
}