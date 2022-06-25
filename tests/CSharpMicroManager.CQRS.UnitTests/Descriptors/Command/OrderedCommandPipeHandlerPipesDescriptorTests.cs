using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Pipelines.Command;
using CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;
using CSharpMicroManager.Functional.Core;
using FluentAssertions;
using NUnit.Framework;

namespace CSharpMicroManager.CQRS.UnitTests.Descriptors.Command;

[TestFixture]
public class OrderedCommandPipeHandlerPipesDescriptorTests
{
    [Test]
    public void WithNext_ShouldAppendPipes_InCalledOrder()
    {
        var descriptor = new OrderedCommandHandlerPipesDescriptor(d => d
            .SetupHandlers(v => v
                .WithNext(typeof(FirstHandlerPipe<>))
                .WithNext(typeof(SecondHandlerPipe<>))
                .WithNext(typeof(ThirdHandlerPipe<>))));

        var first = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Skip(0).Take(1).Single().Value;
        var second = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Skip(1).Take(1).Single().Value;
        var third = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Skip(2).Take(1).Single().Value;

        first.ServiceType.Should().Be(typeof(ICommandHandlerPipe<>));
        second.ServiceType.Should().Be(typeof(ICommandHandlerPipe<>));
        third.ServiceType.Should().Be(typeof(ICommandHandlerPipe<>));
        
        first.ImplementationType.Should().Be(typeof(FirstHandlerPipe<>));
        second.ImplementationType.Should().Be(typeof(SecondHandlerPipe<>));
        third.ImplementationType.Should().Be(typeof(ThirdHandlerPipe<>));
    }
    
    [Test]
    public void WithNext_ShouldAppendCommandHandlingPipe_OnTheEnd()
    {
        var descriptor = new OrderedCommandHandlerPipesDescriptor(d => d
            .SetupHandlers(v => v
                .WithNext(typeof(FirstHandlerPipe<>))
                .WithNext(typeof(SecondHandlerPipe<>))
                .WithNext(typeof(ThirdHandlerPipe<>))));

        var last = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Last().Value;

        last.ServiceType.Should().Be(typeof(ICommandHandlerPipe<>));
        last.ImplementationType.Should().Be(typeof(CommandHandlingPipe<>));
    }

    class FirstHandlerPipe<TCommand> : ICommandHandlerPipe<TCommand> where TCommand : ICommand
    {
        public Task<Result<Unit>> Handle(
            TCommand command, 
            CommandHandlerPipelineDelegate<TCommand> next,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
    class SecondHandlerPipe<TCommand> : ICommandHandlerPipe<TCommand> where TCommand : ICommand
    {
        public Task<Result<Unit>> Handle(
            TCommand command, 
            CommandHandlerPipelineDelegate<TCommand> next,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
    
    class ThirdHandlerPipe<TCommand> : ICommandHandlerPipe<TCommand> where TCommand : ICommand
    {
        public Task<Result<Unit>> Handle(
            TCommand command,
            CommandHandlerPipelineDelegate<TCommand> next,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
    
}