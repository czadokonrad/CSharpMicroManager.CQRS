using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Queries;

public interface IStreamingQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    IAsyncEnumerable<Result<Option<TResult>>> Handle(TQuery query, CancellationToken cancellationToken);
}