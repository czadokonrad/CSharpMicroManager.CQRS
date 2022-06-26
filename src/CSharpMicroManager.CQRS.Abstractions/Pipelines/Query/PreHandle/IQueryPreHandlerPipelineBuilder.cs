using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;

public interface IQueryPreHandlerPipelineBuilder<TQuery, TResult> where TQuery : IQuery<TResult>
{
    IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryPreHandlerPipelineDelegate<TQuery, TResult>, QueryPreHandlerPipelineDelegate<TQuery, TResult>> middleware);
    IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryPreHandlerPipe<TQuery, TResult> pipe);
    IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe<TQueryPreHandlerPipe>() 
        where TQueryPreHandlerPipe : class, IQueryPreHandlerPipe<TQuery, TResult>, new();
    
    QueryPreHandlerPipelineDelegate<TQuery, TResult> Build(IEnumerable<IQueryPreHandlerPipe<TQuery, TResult>> pipes);
    QueryPreHandlerPipelineDelegate<TQuery, TResult> Build();
    IReadOnlyCollection<Func<QueryPreHandlerPipelineDelegate<TQuery, TResult>, QueryPreHandlerPipelineDelegate<TQuery, TResult>>> Pipes { get; }
}