using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public class OrderedQueryHandlerPipesDescriptor : OrderedQueryPipeHandlerPipesDescriptor
{
    public OrderedQueryHandlerPipesDescriptor(QueryPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
        PutQueryHandlingPipeOnEnd();
    }
    
    public override OrderedQueryHandlerPipesDescriptor WithNext(Type pipeType)
    {
        base.WithNext(pipeType);
        return PutQueryHandlingPipeOnEnd();
    }
    
    public OrderedQueryHandlerPipesDescriptor AfterQueryHandlingPipe(Type pipeType)
    {
        PutQueryHandlingPipeOnEnd();
        base.WithNext(pipeType);
        return this;
    }

    private OrderedQueryHandlerPipesDescriptor PutQueryHandlingPipeOnEnd()
    {
        if (SortedPipes.Values.Any(v => v.ImplementationType == typeof(QueryHandlingPipe<,>)))
        {
            SortedPipes.Remove(GetKeyByImplementationType(typeof(QueryHandlingPipe<,>)));
        }

        var last = GetLastKey();
        //Add QueryHandlingPipe as last IQueryHandlerPipe, so IQueryHandler will be executed inside oll registered IQueryHandlerPipe
        SortedPipes.Add(++last, new ServiceDescriptor(
            serviceType: typeof(IQueryHandlerPipe<,>), 
            implementationType: typeof(QueryHandlingPipe<,>),
            ServiceLifetime.Scoped));
        return this;
    }
}