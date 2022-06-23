using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result<Unit>> Handle(TCommand command, CancellationToken cancellationToken);
}