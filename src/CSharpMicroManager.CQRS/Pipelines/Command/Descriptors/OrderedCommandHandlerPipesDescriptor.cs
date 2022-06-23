using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;

public sealed class OrderedCommandHandlerPipesDescriptor : OrderedCommandPipeHandlerPipesDescriptor
{
    public OrderedCommandHandlerPipesDescriptor(CommandPipelinesDescriptor pipelinesDescriptor) : base(pipelinesDescriptor)
    {
        
    }

    public OrderedCommandHandlerPipesDescriptor(Func<CommandPipelinesDescriptor, CommandPipelinesDescriptor> builder) 
        : base(builder(new CommandPipelinesDescriptor()))
    {
        PutCommandHandlingPipeOnEnd();
    }
    
    public override OrderedCommandHandlerPipesDescriptor WithNext(Type pipeType)
    {
        base.WithNext(pipeType);
        return PutCommandHandlingPipeOnEnd();
    }
    
    public OrderedCommandHandlerPipesDescriptor AfterCommandHandler(Type pipeType)
    {
         PutCommandHandlingPipeOnEnd();
         base.WithNext(pipeType);
         return this;
    }

    private OrderedCommandHandlerPipesDescriptor PutCommandHandlingPipeOnEnd()
    {
        if (SortedPipes.Values.Any(v => v.ImplementationType == typeof(CommandHandlingPipe<>)))
        {
            SortedPipes.Remove(GetKeyByImplementationType(typeof(CommandHandlingPipe<>)));
        }

        var last = GetLastKey();
        //Add CommandHandlingPipe as last ICommandHandlerPipe, so ICommandHandler will be executed inside oll registered ICommandHandlerPipe
        SortedPipes.Add(++last, new ServiceDescriptor(
            serviceType: typeof(ICommandHandlerPipe<>), 
            implementationType: typeof(CommandHandlingPipe<>),
            ServiceLifetime.Scoped));
        return this;
    }
}
