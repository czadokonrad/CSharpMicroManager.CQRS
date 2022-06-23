using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;

/// <summary>
/// RePostsents a functionality of QueryPostHandlerPipe, which will be executed after processing actual <see cref="IQueryHandler{TQuery,TResult}"/> on <see cref="IQuery{TResult}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// <typeparam name="TResult">Type of query result</typeparam>
public interface IQueryPostHandlerPipe<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles single step of registered QueryPostHandlerPipeline by using <see cref="IQueryPostHandlerPipelineBuilder{TQuery,TResult}"/>
    /// </summary>
    /// <param name="next">Next handler of type <see cref="IQueryPostHandlerPipe{TQuery, TResult}"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<Result<Option<TResult>>> Handle(
        TQuery query,
        QueryPostHandlerPipelineDelegate<TQuery, TResult> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Delegate which represents a pipeline of registered <see cref="IQueryPostHandlerPipe{TQuery, TResult}"/>
/// </summary>
/// <typeparam name="TQuery">Concrete type of <see cref="IQuery{TResult}"/></typeparam>
/// <typeparam name="TResult">Type of query result</typeparam>
public delegate Task<Result<Option<TResult>>> QueryPostHandlerPipelineDelegate<TQuery, TResult>(
    TQuery query,
    CancellationToken cancellationToken) where TQuery : IQuery<TResult>;