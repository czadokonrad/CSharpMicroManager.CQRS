using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;

/// <summary>
/// Represents mechanics for dispatching <see cref="IQuery{TResult}"/>
/// </summary>
public interface IQueryDispatcher
{
    
    Task<Result<Option<TResult>>> Handle<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>;
}