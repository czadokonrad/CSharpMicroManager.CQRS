using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public class QueryPipelinesDescriptor
{
    public readonly IServiceCollection Services;
    private OrderedQueryPipeHandlerPipesDescriptor? _preHandlerPipesDescriptor;
    public OrderedQueryPipeHandlerPipesDescriptor PreHandlerPipesDescriptor
    {
        get => _preHandlerPipesDescriptor ??= new OrderedQueryPreHandlerPipesDescriptor(this);
        private set => _preHandlerPipesDescriptor = value;
    }

    private OrderedQueryPipeHandlerPipesDescriptor? _handlerPipesDescriptor;
    public OrderedQueryPipeHandlerPipesDescriptor HandlerPipesDescriptor
    {
        get => _handlerPipesDescriptor ??= new OrderedQueryHandlerPipesDescriptor(this);
        private set => _handlerPipesDescriptor = value;
    }

    private OrderedQueryPipeHandlerPipesDescriptor? _postHandlerPipesDescriptor;
    public OrderedQueryPipeHandlerPipesDescriptor PostHandlerPipesDescriptor
    {
        get => _postHandlerPipesDescriptor ??= new OrderedQueryPostHandlerPipesDescriptor(this);
        private set => _postHandlerPipesDescriptor = value;
    }

    public QueryPipelinesDescriptor(IServiceCollection services)
    {
        Services = services;
    }

    public QueryPipelinesDescriptor SetupPreHandlers(
        Func<OrderedQueryPreHandlerPipesDescriptor, OrderedQueryPipeHandlerPipesDescriptor> preHandlerDescriptorFunc)
    {
        PreHandlerPipesDescriptor = preHandlerDescriptorFunc(new OrderedQueryPreHandlerPipesDescriptor(this));
        return this;
    }

    public QueryPipelinesDescriptor SetupHandlers(
        Func<OrderedQueryHandlerPipesDescriptor, OrderedQueryPipeHandlerPipesDescriptor> handlerDescriptorFunc)
    {
        HandlerPipesDescriptor = handlerDescriptorFunc(new OrderedQueryHandlerPipesDescriptor(this));
        return this;
    }

    public QueryPipelinesDescriptor SetupPostHandlers(
        Func<OrderedQueryPostHandlerPipesDescriptor, OrderedQueryPipeHandlerPipesDescriptor>
            postHandlerDescriptorFunc)
    {
        PostHandlerPipesDescriptor = postHandlerDescriptorFunc(new OrderedQueryPostHandlerPipesDescriptor(this));
        return this;
    }
}