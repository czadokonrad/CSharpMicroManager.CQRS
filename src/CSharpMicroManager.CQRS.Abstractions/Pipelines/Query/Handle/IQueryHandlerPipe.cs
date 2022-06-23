using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;

/// <summary>
/// Represents a functionality of QueryHandlerPipe, which will be executed during processing actual <see cref="IQueryHandlerPipe{TQuery, TResult}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// /// <typeparam name="TResult">Type of result/></typeparam>
public interface IQueryHandlerPipe<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles single step of registered QueryHandlerPipeline by using <see cref="IQueryHandlerPipelineBuilder{TQuery, TResult}"/>
    /// </summary>
    /// <param name="next">Next handler of type <see cref="IQueryHandlerPipe{TQuery, TResult}"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<Result<Option<TResult>>> Handle(
        TQuery query,
        QueryHandlerPipelineDelegate<TQuery, TResult> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Delegate which represents a pipeline of registered <see cref="IQueryHandlerPipe{TQuery, TResult}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// /// <typeparam name="TResult">Type of result/></typeparam>
public delegate Task<Result<Option<TResult>>> QueryHandlerPipelineDelegate<TQuery, TResult>(
    TQuery query,
    CancellationToken cancellationToken) where TQuery : IQuery<TResult>;