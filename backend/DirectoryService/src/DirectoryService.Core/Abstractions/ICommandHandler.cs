using CSharpFunctionalExtensions;

namespace DirectoryService.Core.Abstractions;

#pragma warning disable CA1040 // Avoid empty interfaces
public interface ICommand;
#pragma warning restore CA1040

public interface ICommandHandler<TResponse, in TCommand>
    where TCommand : ICommand
{
    Task<Result<TResponse, Shared.Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Shared.Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}