using System.Data;
using System.Net;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.Functional.Core;
using Polly;
using Polly.Wrap;

namespace CSharpMicroManager.CQRS.Dispatching.Command;

public class CommandDispatcherWithRetry : ICommandDispatcher
{
    private readonly CommandDispatcherRetryPolicyDescriptor _dispatcherRetryPolicy;
    private readonly ICommandDispatcher _decorated;

    public CommandDispatcherWithRetry(
        CommandDispatcherRetryPolicyDescriptor dispatcherRetryPolicy,
        ICommandDispatcher decorated)
    {
        _dispatcherRetryPolicy = dispatcherRetryPolicy;
        _decorated = decorated;
    }

    public Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand =>
        _dispatcherRetryPolicy.Policies.ExecuteAsync(() => _decorated.Handle(command, cancellationToken));
}

public class CommandDispatcherRetryPolicyDescriptor
{
    private AsyncPolicyWrap _policies = null!;
    public AsyncPolicyWrap Policies
    {
        get => _policies ??= Policy.WrapAsync(
            Policy.Handle<HttpRequestException>(req => req.StatusCode is HttpStatusCode.ServiceUnavailable).RetryAsync(retryCount: 5),
            Policy.Handle<DBConcurrencyException>().RetryAsync(retryCount: 5));
        set => _policies = value;
    }
}