using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

internal sealed class QueryPipelineBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public QueryPipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public QueryPreHandlerPipelineDelegate<TQuery, TResult> ForPreHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        var preHandlerPipeLineBuilder = _serviceProvider.GetRequiredService<IQueryPreHandlerPipelineBuilder<TQuery, TResult>>();
        var preHandlerProviders = _serviceProvider.GetRequiredService<IEnumerable<IQueryPreHandlerPipe<TQuery, TResult>>>();

        return preHandlerProviders
            .Aggregate(preHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (query, ct) => pipe.Handle(query, next, ct); });
            })
            .Build();
    }
    
    public QueryHandlerPipelineDelegate<TQuery, TResult> ForHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        var handlerPipeLineBuilder = _serviceProvider.GetRequiredService<IQueryHandlerPipelineBuilder<TQuery, TResult>>();
        var handlerProviders = _serviceProvider.GetRequiredService<IEnumerable<IQueryHandlerPipe<TQuery, TResult>>>();
       
        return handlerProviders
            .Aggregate(handlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (query, ct) => pipe.Handle(query, next, ct); });
            })
            .Build();
    }
    
    public QueryPostHandlerPipelineDelegate<TQuery, TResult> ForPostHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        var postHandlerPipeLineBuilder = _serviceProvider.GetRequiredService<IQueryPostHandlerPipelineBuilder<TQuery, TResult>>();
        var postHandlerProviders = _serviceProvider.GetRequiredService<IEnumerable<IQueryPostHandlerPipe<TQuery, TResult>>>();
      
        return postHandlerProviders
            .Aggregate(postHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (query, ct) => pipe.Handle(query, next, ct); });
            })
            .Build();
    }
}