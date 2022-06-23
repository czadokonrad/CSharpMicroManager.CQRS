using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;

public abstract class OrderedCommandPipeHandlerPipesDescriptor
{
    public readonly SortedDictionary<int, ServiceDescriptor> SortedPipes = new();

    public readonly CommandPipelinesDescriptor PipelinesDescriptor;

    protected OrderedCommandPipeHandlerPipesDescriptor(CommandPipelinesDescriptor pipelinesDescriptor)
    {
        PipelinesDescriptor = pipelinesDescriptor;
    }
    
    public virtual OrderedCommandPipeHandlerPipesDescriptor WithNext(Type pipeType)
    {
        Type serviceType;

        if (!pipeType.IsGenericTypeDefinition)
        {
            var interfaceType = pipeType
                .GetInterfaces()
                .Single(i => i.GetGenericTypeDefinition() == typeof(ICommandPreHandlerPipe<>) ||
                                 i.GetGenericTypeDefinition() == typeof(ICommandHandlerPipe<>) || 
                                 i.GetGenericTypeDefinition() == typeof(ICommandPostHandlerPipe<>));
            var genericArg = interfaceType.GetGenericArguments().Single();
            serviceType = interfaceType.GetGenericTypeDefinition().MakeGenericType(genericArg);
        }
        else
        {
            serviceType = pipeType
                .GetInterfaces()
                .Single(i => i.GetGenericTypeDefinition() == typeof(ICommandPreHandlerPipe<>) ||
                             i.GetGenericTypeDefinition() == typeof(ICommandHandlerPipe<>) ||
                             i.GetGenericTypeDefinition() == typeof(ICommandPostHandlerPipe<>))
                .GetGenericTypeDefinition();
        }

        var last = GetLastKey();
        SortedPipes.Add(++last, new ServiceDescriptor(serviceType: serviceType, implementationType: pipeType, ServiceLifetime.Transient));
        return this;
    }

    protected int GetLastKey() => SortedPipes.LastOrDefault().Key;

    protected int GetKeyByImplementationType(Type type) => SortedPipes.Single(v => v.Value.ImplementationType == type).Key;
}