using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Positions.Commands.UpdatePositionName;

public class UpdatePositionNameValidator: AbstractValidator<UpdatePositionNameRequest>
{
  public UpdatePositionNameValidator()
  {
    RuleFor(p => p.Name)
      .MustBeValueObject(PositionName.Create);
  }
}