using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;
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
            .AddQueryHandler<TestQuery, bool, TestQueryHandler>()
            .AddQueryPipelines(d => d
                .SetupPreHandlers(v => v
                    .WithNext(typeof(TestQueryPreHandler)))
                .SetupPostHandlers(v => v
                    .WithNext(typeof(TestQueryPostHandler))));

        var serviceProvider = services.BuildServiceProvider();

        var testCounter = serviceProvider.GetRequiredService<TestCounter>();
        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        await dispatcher.Handle<TestQuery, bool>(new TestQuery(), default);

        testCounter.InvokeCount.Should().Be(3);
    }


    class TestQuery : IQuery<bool>
    {
        
    }

    class TestQueryPreHandler : IQueryPreHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;

        public TestQueryPreHandler(TestCounter counter)
        {
            _counter = counter;
        }

        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryPreHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
        }
    }
    
    class TestQueryHandler : IQueryHandler<TestQuery, bool>
    {
        private readonly TestCounter _counter;

        public TestQueryHandler(TestCounter counter)
        {
            _counter = counter;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
        }
    }
    
    class TestQueryPostHandler : IQueryPostHandlerPipe<TestQuery, bool>
    {
        private readonly TestCounter _counter;

        public TestQueryPostHandler(TestCounter counter)
        {
            _counter = counter;
        }
        public Task<Result<Option<bool>>> Handle(TestQuery query, QueryPostHandlerPipelineDelegate<TestQuery, bool> next, CancellationToken cancellationToken)
        {
            ++_counter.InvokeCount;
            return Task.FromResult(new Result<Option<bool>>(F.None));
        }
    }
    
    class TestCounter
    {
        public int InvokeCount { get; set; }
    }
}