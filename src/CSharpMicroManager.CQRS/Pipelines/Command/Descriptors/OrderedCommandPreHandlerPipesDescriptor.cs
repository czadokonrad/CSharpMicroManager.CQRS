namespace CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;

public sealed class OrderedCommandPreHandlerPipesDescriptor : OrderedCommandPipeHandlerPipesDescriptor
{
    public OrderedCommandPreHandlerPipesDescriptor(CommandPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
    }

    public override OrderedCommandPreHandlerPipesDescriptor WithNext(Type pipeType)
    { 
        base.WithNext(pipeType);
        return this;
    }
}