using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

public abstract class OrderedQueryPipeHandlerPipesDescriptor
{
    public readonly SortedDictionary<int, ServiceDescriptor> SortedPipes = new();

    private readonly QueryPipelinesDescriptor _pipelinesDescriptor;

    protected OrderedQueryPipeHandlerPipesDescriptor(QueryPipelinesDescriptor pipelinesDescriptor)
    {
        _pipelinesDescriptor = pipelinesDescriptor;
    }
    
    public virtual OrderedQueryPipeHandlerPipesDescriptor WithNext(Type pipeType)
    {
        Type serviceType;

        if (!pipeType.IsGenericTypeDefinition)
        {
            var interfaceType = pipeType
                .GetInterfaces()
                .Single(i => i.GetGenericTypeDefinition() == typeof(IQueryPreHandlerPipe<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IQueryHandlerPipe<,>) || 
                             i.GetGenericTypeDefinition() == typeof(IQueryPostHandlerPipe<,>));
            var genericArgs = interfaceType.GetGenericArguments().Take(2).ToList();
            serviceType = interfaceType.GetGenericTypeDefinition().MakeGenericType(genericArgs[0], genericArgs[1]);
        }
        else
        {
            serviceType = pipeType
                .GetInterfaces()
                .Single(i => i.GetGenericTypeDefinition() == typeof(IQueryPreHandlerPipe<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IQueryHandlerPipe<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IQueryPostHandlerPipe<,>))
                .GetGenericTypeDefinition();
        }

        var last = GetLastKey();
        SortedPipes.Add(++last, new ServiceDescriptor(serviceType: serviceType, implementationType: pipeType, ServiceLifetime.Scoped));
        return this;
    }

    protected int GetLastKey() => SortedPipes.LastOrDefault().Key;

    protected int GetKeyByImplementationType(Type type) => SortedPipes.Single(v => v.Value.ImplementationType == type).Key;
}