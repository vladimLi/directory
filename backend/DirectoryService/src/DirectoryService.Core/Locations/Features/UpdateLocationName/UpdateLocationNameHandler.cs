using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Features.UpdateLocationName;

public class UpdateLocationNameHandler : ICommandHandler<Guid, UpdateLocationNameCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<UpdateLocationNameHandler> _logger;
    private readonly IValidator<UpdateLocationNameRequest> _validator;

    public UpdateLocationNameHandler(
        ILocationsRepository repository,
        ILogger<UpdateLocationNameHandler> logger,
        IValidator<UpdateLocationNameRequest> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }
    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateLocationNameCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationId = LocationId.Create(command.Request.Id);

        var location = await _repository.GetByIdAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
            return location.Error;

        var result = location.Value.UpdateName(command.Request.Name);

        if (result.IsFailure)
            return result.Error;

        var saveResult = await _repository.Save(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Update location name {LocationId}", location.Value.Id.Value);
        return location.Value.Id.Value;
    }
}