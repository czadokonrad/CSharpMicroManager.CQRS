using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Queries;

/// <summary>
/// Represents functionality for handling <see cref="IQuery{TResult}"/> to return some data
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Returns data requested by specific <see cref="IQuery{TResult}"/>
    /// </summary>
    /// <param name="query">Query which contains information about requested data</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns><see cref="Result{T}"/> which contains information about request error or optional result in <see cref="Option{T}"/></returns>
    Task<Result<Option<TResult>>> Handle(TQuery query, CancellationToken cancellationToken);
}
