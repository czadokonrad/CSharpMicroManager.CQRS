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
public class OrderedCommandHandlerPipesDescriptorTests
{
    [Test]
    public void AfterCommandHandler_ShouldAppendCommandPipe_AfterCommandHandlingPipe()
    {
        var descriptor = new OrderedCommandHandlerPipesDescriptor(d => d
            .SetupHandlers(v => v
                .WithNext(typeof(FirstHandlerPipe<>))
                .WithNext(typeof(SecondHandlerPipe<>))
                .AfterCommandHandler(typeof(LastHandlerPipe<>))));
        
        var beforeLast = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes
            .Skip(descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Count - 2)
            .Take(1)
            .Single().Value;
        
        var last = descriptor.PipelinesDescriptor.HandlerPipesDescriptor.SortedPipes.Last().Value;

        beforeLast.ImplementationType.Should().Be(typeof(CommandHandlingPipe<>));
        last.ImplementationType.Should().Be(typeof(LastHandlerPipe<>));
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
    
    class LastHandlerPipe<TCommand> : ICommandHandlerPipe<TCommand> where TCommand : ICommand
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