using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;

public sealed class CommandPipelinesDescriptor
{
    public IServiceCollection? Services;
    private OrderedCommandPipeHandlerPipesDescriptor? _preHandlerPipesDescriptor;

    public CommandPipelinesDescriptor WithServices(IServiceCollection services)
    {
        Services = services;
        return this;
    }
    
    public OrderedCommandPipeHandlerPipesDescriptor PreHandlerPipesDescriptor
    {
        get => _preHandlerPipesDescriptor ??= new OrderedCommandPreHandlerPipesDescriptor(this);
        private set => _preHandlerPipesDescriptor = value;
    }

    private OrderedCommandPipeHandlerPipesDescriptor? _handlerPipesDescriptor;
    public OrderedCommandPipeHandlerPipesDescriptor HandlerPipesDescriptor
    {
        get => _handlerPipesDescriptor ??= new OrderedCommandHandlerPipesDescriptor(this);
        private set => _handlerPipesDescriptor = value;
    }

    private OrderedCommandPipeHandlerPipesDescriptor? _postHandlerPipesDescriptor;
    public OrderedCommandPipeHandlerPipesDescriptor PostHandlerPipesDescriptor
    {
        get => _postHandlerPipesDescriptor ??= new OrderedCommandPostHandlerPipesDescriptor(this);
        private set => _postHandlerPipesDescriptor = value;
    }
    
    public CommandPipelinesDescriptor SetupPreHandlers(
        Func<OrderedCommandPreHandlerPipesDescriptor, OrderedCommandPipeHandlerPipesDescriptor> preHandlerDescriptorFunc)
    {
        PreHandlerPipesDescriptor = preHandlerDescriptorFunc(new OrderedCommandPreHandlerPipesDescriptor(this));
        return this;
    }

    public CommandPipelinesDescriptor SetupHandlers(
        Func<OrderedCommandHandlerPipesDescriptor, OrderedCommandPipeHandlerPipesDescriptor> handlerDescriptorFunc)
    {
        HandlerPipesDescriptor = handlerDescriptorFunc(new OrderedCommandHandlerPipesDescriptor(this));
        return this;
    }

    public CommandPipelinesDescriptor SetupPostHandlers(
        Func<OrderedCommandPostHandlerPipesDescriptor, OrderedCommandPipeHandlerPipesDescriptor>
            postHandlerDescriptorFunc)
    {
        PostHandlerPipesDescriptor = postHandlerDescriptorFunc(new OrderedCommandPostHandlerPipesDescriptor(this));
        return this;
    }
}