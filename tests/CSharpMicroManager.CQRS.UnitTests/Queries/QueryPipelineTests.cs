using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.CQRS.Extensions;
using CSharpMicroManager.Functional.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CSharpMicroManager.CQRS.UnitTests.Queries;

[TestFixture]
public class QueryPipelineTests
{
    [Test]
    public async Task Pipeline_ShouldExecute_AllRegistered_Pipes()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TestCounter>()
            .AddSingleton<TestInvokeOrderVerifier>()
            .AddQueryHandler<TestQuery, bool, TestQueryHandler>()
            .AddQueryPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestQueryPreHandler)))
                .SetupHandlers(v => v
                    .WithNext(typeof(TestQueryHandlerPipe)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestQueryPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var testCounter = serviceProvider.GetRequiredService<TestCounter>();
        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        await dispatcher.Handle<TestQuery, bool>(new TestQuery(), default);

        testCounter.InvokeCount.Should().Be(4);
    }

    
    [Test]
    public async Task Pipeline_ShouldExecute_AllRegistered_Pipes_InDefinedOrder()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<TestCounter>()
            .AddSingleton<TestInvokeOrderVerifier>()
            .AddQueryHandler<TestQuery, bool, TestQueryHandler>()
            .AddQueryPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestQueryPreHandler)))
                .SetupHandlers(v => v
                    .WithNext(typeof(TestQueryHandlerPipe))
                    .AfterQueryHandlingPipe(typeof(TestQueryHandlerPipeAfterHandler)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestQueryPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var invokeOrderVerifier = serviceProvider.GetRequiredService<TestInvokeOrderVerifier>();
        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        await dispatcher.Handle<TestQuery, bool>(new TestQuery(), default);

        invokeOrderVerifier.Next().Should().Be(typeof(TestQueryPreHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestQueryHandlerPipe));
        invokeOrderVerifier.Next().Should().Be(typeof(TestQueryHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestQueryHandlerPipeAfterHandler));
        invokeOrderVerifier.Next().Should().Be(typeof(TestQueryPostHandler));
    }

    class TestQuery : IQuery<bool>
    {
        
    }

    class TestQueryPreHandler : IQueryPreHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _orderVerifier;

        public TestQueryPreHandler(
            TestCounter counter,
            TestInvokeOrderVerifier orderVerifier)
        {
            _counter = counter;
            _orderVerifier = orderVerifier;
        }

        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryPreHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            _orderVerifier.RegisterInvoke(typeof(TestQueryPreHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
        }
    }
    
    class TestQueryHandlerPipe : IQueryHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _orderVerifier;

        public TestQueryHandlerPipe(
            TestCounter counter,
            TestInvokeOrderVerifier orderVerifier)
        {
            _counter = counter;
            _orderVerifier = orderVerifier;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            _orderVerifier.RegisterInvoke(typeof(TestQueryHandlerPipe));
            ++_counter.InvokeCount;
            return next(query, cancellationToken);
        }
    }
    
    class TestQueryHandlerPipeAfterHandler : IQueryHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _orderVerifier;

        public TestQueryHandlerPipeAfterHandler(
            TestCounter counter,
            TestInvokeOrderVerifier orderVerifier)
        {
            _counter = counter;
            _orderVerifier = orderVerifier;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            _orderVerifier.RegisterInvoke(typeof(TestQueryHandlerPipeAfterHandler));
            ++_counter.InvokeCount;
            return next(query, cancellationToken);
        }
    }
    
    class TestQueryHandler : IQueryHandler<TestQuery, bool>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _orderVerifier;

        public TestQueryHandler(
            TestCounter counter,
            TestInvokeOrderVerifier orderVerifier)
        {
            _counter = counter;
            _orderVerifier = orderVerifier;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            _orderVerifier.RegisterInvoke(typeof(TestQueryHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
        }
    }
    
    class TestQueryPostHandler : IQueryPostHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;
        private readonly TestInvokeOrderVerifier _orderVerifier;

        public TestQueryPostHandler(
            TestCounter counter,
            TestInvokeOrderVerifier orderVerifier)
        {
            _counter = counter;
            _orderVerifier = orderVerifier;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryPostHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            _orderVerifier.RegisterInvoke(typeof(TestQueryPostHandler));
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
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