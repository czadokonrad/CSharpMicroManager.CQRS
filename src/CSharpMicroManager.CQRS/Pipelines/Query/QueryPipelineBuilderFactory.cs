using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

public class QueryPipelineBuilderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public QueryPipelineBuilderFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public QueryHandlerPipelineWrapper<TQuery, TResult> CreatePipeline<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        return new QueryHandlerPipelineWrapper<TQuery, TResult>(
            GetQueryPreHandler<TQuery, TResult>(_serviceProvider),
            GetQueryHandler<TQuery, TResult>(_serviceProvider),
            GetQueryPostHandler<TQuery, TResult>(_serviceProvider));

    }
    
     private QueryPreHandlerPipelineDelegate<TQuery, TResult> GetQueryPreHandler<TQuery, TResult>(IServiceProvider serviceProvider) 
         where TQuery : IQuery<TResult>
    {
        IQueryPreHandlerPipelineBuilder<TQuery, TResult> preHandlerPipeLineBuilder = new QueryPreHandlerPipelineBuilder<TQuery, TResult>();
        var preHandlerProviders = serviceProvider.GetRequiredService<IEnumerable<IQueryPreHandlerPipe<TQuery, TResult>>>();
        
        return preHandlerProviders
            .Aggregate(preHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    private QueryHandlerPipelineDelegate<TQuery, TResult> GetQueryHandler<TQuery, TResult>(IServiceProvider serviceProvider)
        where TQuery : IQuery<TResult>
    {
        IQueryHandlerPipelineBuilder<TQuery, TResult> handlerPipeLineBuilder = new QueryHandlerPipelineBuilder<TQuery, TResult>();
        var handlerProviders = serviceProvider.GetRequiredService<IEnumerable<IQueryHandlerPipe<TQuery, TResult>>>();
        
        return handlerProviders
            .Aggregate(handlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    private QueryPostHandlerPipelineDelegate<TQuery, TResult> GetQueryPostHandler<TQuery, TResult>(IServiceProvider serviceProvider) 
        where TQuery : IQuery<TResult>
    {
        IQueryPostHandlerPipelineBuilder<TQuery, TResult> postHandlerPipeLineBuilder = new QueryPostHandlerPipelineBuilder<TQuery, TResult>();
        var postHandlerProviders = serviceProvider.GetRequiredService<IEnumerable<IQueryPostHandlerPipe<TQuery, TResult>>>();
        
        return postHandlerProviders
            .Aggregate(postHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
}