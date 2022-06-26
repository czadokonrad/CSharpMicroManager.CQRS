using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

internal sealed class QueryPipelineBuilderFactory<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly IQueryPreHandlerPipelineBuilder<TQuery, TResult> _preHandlerPipelineBuilder;
    private readonly IQueryHandlerPipelineBuilder<TQuery, TResult> _handlerPipelineBuilder;
    private readonly IQueryPostHandlerPipelineBuilder<TQuery, TResult> _postHandlerPipelineBuilder;
    private readonly IEnumerable<IQueryPreHandlerPipe<TQuery, TResult>> _queryPreHandlerPipes;
    private readonly IEnumerable<IQueryHandlerPipe<TQuery, TResult>> _queryHandlerPipes;
    private readonly IEnumerable<IQueryPostHandlerPipe<TQuery, TResult>> _queryPostHandlerPipes;
    private readonly ILoggerFactory _loggerFactory;

    public QueryPipelineBuilderFactory(
        IQueryPreHandlerPipelineBuilder<TQuery, TResult> preHandlerPipelineBuilder,
        IQueryHandlerPipelineBuilder<TQuery, TResult> handlerPipelineBuilder,
        IQueryPostHandlerPipelineBuilder<TQuery, TResult> postHandlerPipelineBuilder,
        IEnumerable<IQueryPreHandlerPipe<TQuery, TResult>> queryPreHandlerPipes,
        IEnumerable<IQueryHandlerPipe<TQuery, TResult>> queryHandlerPipes,
        IEnumerable<IQueryPostHandlerPipe<TQuery, TResult>> queryPostHandlerPipes,
        ILoggerFactory loggerFactory)

    {
        _preHandlerPipelineBuilder = preHandlerPipelineBuilder;
        _handlerPipelineBuilder = handlerPipelineBuilder;
        _postHandlerPipelineBuilder = postHandlerPipelineBuilder;
        _queryPreHandlerPipes = queryPreHandlerPipes;
        _queryHandlerPipes = queryHandlerPipes;
        _queryPostHandlerPipes = queryPostHandlerPipes;
        _loggerFactory = loggerFactory;
    }

    public QueryHandlerPipelineWrapper<TQuery, TResult> CreatePipeline() =>
        new(
            _preHandlerPipelineBuilder.Build(_queryPreHandlerPipes),
            _handlerPipelineBuilder.Build(_queryHandlerPipes),
            _postHandlerPipelineBuilder.Build(_queryPostHandlerPipes),
            _loggerFactory.CreateLogger<QueryHandlerPipelineWrapper<TQuery, TResult>>());
}