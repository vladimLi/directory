using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Errors>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            context.AddFailure(new FluentValidation.Results.ValidationFailure
            {
                ErrorMessage = result.Error.First().Message,
                ErrorCode = result.Error.First().Code,
                CustomState = result.Error
            });
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Errors errors)
    {
        var first = errors.First();

        return rule
            .WithMessage(first.Message)
            .WithErrorCode(first.Code)
            .WithState(_ => errors);
    }
}