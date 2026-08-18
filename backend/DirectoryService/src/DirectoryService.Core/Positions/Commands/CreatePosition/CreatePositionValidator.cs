using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Positions.Commands.CreatePosition;

public class CreatePositionValidator : AbstractValidator<CreatePositionRequest>
{
    public CreatePositionValidator()
    {
        RuleFor(p => p.Name)
            .MustBeValueObject(PositionName.Create);
    }
}