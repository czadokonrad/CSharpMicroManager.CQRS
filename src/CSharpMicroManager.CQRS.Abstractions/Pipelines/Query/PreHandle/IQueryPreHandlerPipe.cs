using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;

/// <summary>
/// Represents a functionality of QueryPreHandlerPipe, which will be executed before processing actual <see cref="IQueryHandler{TQuery, TResult}"/> on <see cref="IQuery{TResult}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// <typeparam name="TResult">Type of query result</typeparam>
public interface IQueryPreHandlerPipe<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles single step of registered QueryPreHandlerPipeline by using <see cref="IQueryPreHandlerPipelineBuilder{TQuery, TResult}"/>
    /// </summary>
    /// <param name="next">Next handler of type <see cref="IQueryPreHandlerPipe{TQuery, TResult}"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<Result<Option<TResult>>> Handle(
        TQuery query,
        QueryPreHandlerPipelineDelegate<TQuery, TResult> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Delegate which represents a pipeline of registered <see cref="ICommandHandlerPipe{TCommand}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// <typeparam name="TResult">Type of query result</typeparam>
public delegate Task<Result<Option<TResult>>> QueryPreHandlerPipelineDelegate<TQuery, TResult>(
    TQuery query,
    CancellationToken cancellationToken) where TQuery : IQuery<TResult>;