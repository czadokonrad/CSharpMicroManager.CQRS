using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.CQRS.Extensions;
using CSharpMicroManager.Functional.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CSharpMicroManager.CQRS.UnitTests.Commands;

[TestFixture]
public class CommandPipelineTests
{
    [Test]
    public async Task Pipeline_ShouldExecute_AllRegistered_Pipes()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TestCounter>()
            .AddCommandHandler<TestCommand, TestCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestCommandPreHandler)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestCommandPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var testCounter = serviceProvider.GetRequiredService<TestCounter>();
        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        await dispatcher.Handle(new TestCommand(), default);

        testCounter.InvokeCount.Should().Be(3);
    }


    class TestCommand : ICommand
    {
        
    }

    class TestCommandPreHandler : ICommandPreHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;

        public TestCommandPreHandler(TestCounter counter)
        {
            _counter = counter;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CommandPreHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCommandHandler : ICommandHandler<TestCommand>
    {
        private readonly TestCounter _counter;

        public TestCommandHandler(TestCounter counter)
        {
            _counter = counter;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCommandPostHandler : ICommandPostHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;

        public TestCommandPostHandler(TestCounter counter)
        {
            _counter = counter;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CommandPostHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCounter
    {
        public int InvokeCount { get; set; }
    }
}