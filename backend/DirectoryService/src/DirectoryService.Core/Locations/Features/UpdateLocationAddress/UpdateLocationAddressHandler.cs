using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.Features.UpdateLocationAddress;

public class UpdateLocationAddressHandler : ICommandHandler<Guid,UpdateLocationAddressCommand>
{
    private readonly ILocationsRepository _repository;
    private readonly ILogger<UpdateLocationAddressHandler> _logger;
    private readonly IValidator<UpdateLocationAddressRequest> _validator;

    public UpdateLocationAddressHandler(
        ILocationsRepository repository,
        ILogger<UpdateLocationAddressHandler> logger,
        IValidator<UpdateLocationAddressRequest> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<Result<Guid, Shared.Errors>> Handle(
        UpdateLocationAddressCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationId = LocationId.Create(command.Request.Id);

        var location = await _repository.GetByIdAsync(locationId.Value, cancellationToken);
        if (location.IsFailure)
            return location.Error;

        var result =
            location.Value.UpdateAddress(
                command.Request.Address.Street, 
                command.Request.Address.City,
                command.Request.Address.Country);
        
        if (result.IsFailure)
            return result.Error;

        var saveResult = await _repository.Save(cancellationToken);
        if(saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Update location address {LocationId}", location.Value.Id.Value);
        return location.Value.Id.Value;
    }
}