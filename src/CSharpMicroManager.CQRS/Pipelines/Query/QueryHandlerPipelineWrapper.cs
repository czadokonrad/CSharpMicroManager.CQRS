using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

public class QueryHandlerPipelineWrapper<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly QueryPreHandlerPipelineDelegate<TQuery, TResult> _preHandle;
    private readonly QueryHandlerPipelineDelegate<TQuery, TResult> _next;
    private readonly QueryPostHandlerPipelineDelegate<TQuery, TResult> _postHandle;
    private readonly ILogger<QueryHandlerPipelineWrapper<TQuery, TResult>> _logger;

    public QueryHandlerPipelineWrapper(
        QueryPreHandlerPipelineDelegate<TQuery, TResult> preHandle,
        QueryHandlerPipelineDelegate<TQuery, TResult> next,
        QueryPostHandlerPipelineDelegate<TQuery, TResult> postHandle,
        ILogger<QueryHandlerPipelineWrapper<TQuery, TResult>> logger)
    {
        _preHandle = preHandle;
        _next = next;
        _postHandle = postHandle;
        _logger = logger;
    }
    
    public async Task<Result<Option<TResult>>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var result = await _preHandle(query, cancellationToken);

        if (result.Errors.Any())
        {
            return result;
        }
        
        var handlerResult = await _next(query, cancellationToken);

        try
        {
            var postHandlerResult = await _postHandle(query, cancellationToken);
            
            if (postHandlerResult.IsError)
            {
                _logger.LogError("Executing of QueryPostHandlerPipeline for query: {Query} finished with following errors: {Errors}",
                    typeof(TQuery).FullName,
                    string.Join(Environment.NewLine, postHandlerResult.Errors.Select(e => e.Message)));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error during executing QueryPostHandlerPipeline for query: {Query}", typeof(TQuery).FullName);
        }

        return handlerResult;
    }
}