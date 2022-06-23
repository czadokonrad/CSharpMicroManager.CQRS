using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Pipelines.Query.Cache;

internal sealed class QueryHandlerPipelinesCache<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly QueryPipelineBuilder _queryPipelineBuilder;
    private readonly Dictionary<Type, QueryPreHandlerPipelineDelegate<TQuery, TResult>> _preQueryHandlerPipelines = new();
    private readonly Dictionary<Type, QueryHandlerPipelineDelegate<TQuery, TResult>> _queryHandlerPipelines = new();
    private readonly Dictionary<Type, QueryPostHandlerPipelineDelegate<TQuery, TResult>> _postQueryHandlerPipelines = new();

    public QueryHandlerPipelinesCache(QueryPipelineBuilder queryPipelineBuilder)
    {
        _queryPipelineBuilder = queryPipelineBuilder;
    }
    
    public QueryPreHandlerPipelineDelegate<TQuery, TResult> GetPreHandlePipeline()
    {
        if(_preQueryHandlerPipelines.TryGetValue(typeof(TQuery), out var @delegate))
        {
            return @delegate;
        }

        var preHandler = _queryPipelineBuilder.ForPreHandler<TQuery, TResult>();
        
        _preQueryHandlerPipelines.Add(typeof(TQuery), preHandler);
        return preHandler;
    }
    
    public QueryHandlerPipelineDelegate<TQuery, TResult> GetHandlerPipeline()
    {
        if(_queryHandlerPipelines.TryGetValue(typeof(TQuery), out var @delegate))
        {
            return @delegate;
        }

        var handler = _queryPipelineBuilder.ForHandler<TQuery, TResult>();
        
        _queryHandlerPipelines.Add(typeof(TQuery), handler);
        return handler;
    }
    
    public QueryPostHandlerPipelineDelegate<TQuery, TResult> GetPostHandlerPipeline()
    {
        if(_postQueryHandlerPipelines.TryGetValue(typeof(TQuery), out var @delegate))
        {
            return @delegate;
        }

        var postHandler = _queryPipelineBuilder.ForPostHandler<TQuery, TResult>();
        
        _postQueryHandlerPipelines.Add(typeof(TQuery), postHandler);
        return postHandler;
    }
}