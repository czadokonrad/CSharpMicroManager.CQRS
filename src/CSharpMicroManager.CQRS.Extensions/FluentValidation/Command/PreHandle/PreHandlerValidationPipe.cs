using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.CQRS.Errors.Business;
using CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;
using CSharpMicroManager.Functional.Core;
using FluentValidation;

namespace CSharpMicroManager.CQRS.Extensions.FluentValidation.Command.PreHandle;

internal sealed class PreHandlerFluentValidationPipe<TCommand> : ICommandPreHandlerPipe<TCommand>
    where TCommand : ICommand
{
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public PreHandlerFluentValidationPipe(IEnumerable<IValidator<TCommand>> validators)
    {
        _validators = validators;
    }

    public Task<Result<Unit>> Handle(CommandPreHandlerPipelineContext<TCommand> context,
        CommandPreHandlerPipelineDelegate<TCommand> next, CancellationToken cancellationToken)
    {
        var result = _validators.Select(v => v.Validate(context.Command));

        var validationResults = result.ToList();
        if (validationResults.All(r => r.IsValid))
        {
            return next(context, cancellationToken);
        }

        var errors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        return Task.FromResult(errors.Any()
            ? new Result<Unit>(new CommandValidationError(errors))
            : new Result<Unit>(Unit.Value));
    }
}

public static class ServiceCollectionExtensions
{
    public static OrderedCommandPreHandlerPipesDescriptor UseFluentValidationPipe(
        this OrderedCommandPreHandlerPipesDescriptor descriptor)
    {
        descriptor.WithNext(typeof(PreHandlerFluentValidationPipe<>));
        return descriptor;
    }
}