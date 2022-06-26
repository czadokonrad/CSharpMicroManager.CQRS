namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public class OrderedQueryPostHandlerPipesDescriptor : OrderedQueryPipeHandlerPipesDescriptor
{
    public OrderedQueryPostHandlerPipesDescriptor(QueryPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
    }

    public override OrderedQueryPostHandlerPipesDescriptor WithNext(Type pipeType)
    {
         base.WithNext(pipeType);
         return this;
    }
}