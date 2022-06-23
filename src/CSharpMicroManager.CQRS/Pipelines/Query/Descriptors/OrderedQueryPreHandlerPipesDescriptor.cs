namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public class OrderedQueryPreHandlerPipesDescriptor : OrderedQueryHandlerPipesDescriptor
{
    public OrderedQueryPreHandlerPipesDescriptor(QueryPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
    }

    public override OrderedQueryHandlerPipesDescriptor WithNext(Type pipeType)
    { 
        base.WithNext(pipeType);
        return this;
    }
}