namespace CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;

public sealed class OrderedCommandPostHandlerPipesDescriptor : OrderedCommandPipeHandlerPipesDescriptor
{
    public OrderedCommandPostHandlerPipesDescriptor(CommandPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
    }

    public override OrderedCommandPostHandlerPipesDescriptor WithNext(Type pipeType)
    { 
        base.WithNext(pipeType);
        return this;
    }
}