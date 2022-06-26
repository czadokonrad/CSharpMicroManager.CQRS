namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public class OrderedQueryPreHandlerPipesDescriptor : OrderedQueryPipeHandlerPipesDescriptor
{
    public OrderedQueryPreHandlerPipesDescriptor(QueryPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
    }

    public override OrderedQueryPreHandlerPipesDescriptor WithNext(Type pipeType)
    { 
        base.WithNext(pipeType);
        return this;
    }
}