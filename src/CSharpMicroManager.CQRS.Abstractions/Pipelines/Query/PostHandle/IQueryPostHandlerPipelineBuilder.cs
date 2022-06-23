using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;

public interface IQueryPostHandlerPipelineBuilder<TQuery, TResult> where TQuery : IQuery<TResult>
{
    IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryPostHandlerPipelineDelegate<TQuery, TResult>, QueryPostHandlerPipelineDelegate<TQuery, TResult>> middleware);
    IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryPostHandlerPipe<TQuery, TResult> pipe);
    IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe<TCommandPostHandlerPipe>() 
        where TCommandPostHandlerPipe : class, IQueryPostHandlerPipe<TQuery, TResult>, new();
    QueryPostHandlerPipelineDelegate<TQuery, TResult> Build();
    IReadOnlyCollection<Func<QueryPostHandlerPipelineDelegate<TQuery, TResult>, QueryPostHandlerPipelineDelegate<TQuery, TResult>>> Pipes { get; }
}