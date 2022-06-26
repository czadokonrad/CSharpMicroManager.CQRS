using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;

public interface IQueryHandlerPipelineBuilder<TQuery, TResult> where TQuery : IQuery<TResult>
{
    IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryHandlerPipelineDelegate<TQuery, TResult>, QueryHandlerPipelineDelegate<TQuery, TResult>> middleware);
    IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryHandlerPipe<TQuery, TResult> pipe);
    IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe<TCommandHandlerPipe>() 
        where TCommandHandlerPipe : class, IQueryHandlerPipe<TQuery, TResult>, new();
    
    QueryHandlerPipelineDelegate<TQuery, TResult> Build(IEnumerable<IQueryHandlerPipe<TQuery, TResult>> pipes);
    QueryHandlerPipelineDelegate<TQuery, TResult> Build();
    IReadOnlyCollection<Func<QueryHandlerPipelineDelegate<TQuery, TResult>, QueryHandlerPipelineDelegate<TQuery, TResult>>> Pipes { get; }
}