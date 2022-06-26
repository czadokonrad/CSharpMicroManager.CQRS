using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
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
            .AddSingleton<TestInvokeOrderVerifier>()
            .AddCommandHandler<TestCommand, TestCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestCommandPreHandler)))
                .SetupHandlers(v => v
                    .WithNext(typeof(TestCommandHandlerPipe)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestCommandPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var testCounter = serviceProvider.GetRequiredService<TestCounter>();
        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        await dispatcher.Handle(new TestCommand(), default);

        testCounter.InvokeCount.Should().Be(4);
    }

    [Test]
    public async Task Pipeline_ShouldExecute_AllRegistered_Pipes_InDefinedOrder()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TestCounter>()
            .AddSingleton<TestInvokeOrderVerifier>()
            .AddCommandHandler<TestCommand, TestCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestCommandPreHandler)))
                .SetupHandlers(v => v
                    .WithNext(typeof(TestCommandHandlerPipe))
                    .AfterCommandHandler(typeof(TestCommandHandlerPipeAfterHandler)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestCommandPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var invokeOrderVerifier = serviceProvider.GetRequiredService<TestInvokeOrderVerifier>();
        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        await dispatcher.Handle(new TestCommand(), default);

        invokeOrderVerifier.Next().Should().Be(typeof(TestCommandPreHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestCommandHandlerPipe));
        invokeOrderVerifier.Next().Should().Be(typeof(TestCommandHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestCommandHandlerPipeAfterHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestCommandPostHandler));
    }
    

    class TestCommand : ICommand
    {
        
    }

    class TestCommandPreHandler : ICommandPreHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _invokeOrderVerifier;

        public TestCommandPreHandler(
            TestCounter counter,
            TestInvokeOrderVerifier invokeOrderVerifier)
        {
            _counter = counter;
            _invokeOrderVerifier = invokeOrderVerifier;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CommandPreHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            _invokeOrderVerifier.RegisterInvoke(typeof(TestCommandPreHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCommandHandlerPipe : ICommandHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _invokeOrderVerifier;

        public TestCommandHandlerPipe(
            TestCounter counter,
            TestInvokeOrderVerifier invokeOrderVerifier)
        {
            _counter = counter;
            _invokeOrderVerifier = invokeOrderVerifier;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CommandHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            _invokeOrderVerifier.RegisterInvoke(typeof(TestCommandHandlerPipe));
            ++_counter.InvokeCount;
            return next(command, cancellationToken);
        }
    }
    
    class TestCommandHandler : ICommandHandler<TestCommand>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _invokeOrderVerifier;

        public TestCommandHandler(
            TestCounter counter,
            TestInvokeOrderVerifier invokeOrderVerifier)
        {
            _counter = counter;
            _invokeOrderVerifier = invokeOrderVerifier;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            _invokeOrderVerifier.RegisterInvoke(typeof(TestCommandHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCommandHandlerPipeAfterHandler : ICommandHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _invokeOrderVerifier;

        public TestCommandHandlerPipeAfterHandler(
            TestCounter counter,
            TestInvokeOrderVerifier invokeOrderVerifier)
        {
            _counter = counter;
            _invokeOrderVerifier = invokeOrderVerifier;
        }
        public Task<Result<Unit>> Handle(TestCommand command,  CommandHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            _invokeOrderVerifier.RegisterInvoke(typeof(TestCommandHandlerPipeAfterHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCommandPostHandler : ICommandPostHandlerPipe<TestCommand>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _invokeOrderVerifier;

        public TestCommandPostHandler(
            TestCounter counter,
            TestInvokeOrderVerifier invokeOrderVerifier)
        {
            _counter = counter;
            _invokeOrderVerifier = invokeOrderVerifier;
        }
        public Task<Result<Unit>> Handle(TestCommand command, CommandPostHandlerPipelineDelegate<TestCommand> next, CancellationToken cancellationToken)
        {
            _invokeOrderVerifier.RegisterInvoke(typeof(TestCommandPostHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    class TestCounter
    {
        public int InvokeCount { get; set; }
    }
    
    class TestInvokeOrderVerifier
    {
        public SortedDictionary<int, Type> InvokeOrder { get; set; } = new();

        public void RegisterInvoke(Type type)
        {
            var lastKey = InvokeOrder.LastOrDefault().Key;
            lastKey = lastKey == default ? 1 : ++lastKey;
            
            InvokeOrder.Add(lastKey, type);
        }

        private int curr = 1;
        public Type Next()
        {
            return InvokeOrder[curr++];
        }
    }
}