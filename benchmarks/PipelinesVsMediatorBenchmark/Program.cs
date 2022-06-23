// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.CQRS.Extensions;
using CSharpMicroManager.Functional.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PipelinesVsMediatorBenchmark;
using Unit = CSharpMicroManager.Functional.Core.Unit;

BenchmarkRunner.Run<PipelinesVSMediatorBenchmark>();

namespace PipelinesVsMediatorBenchmark
{
    [MemoryDiagnoser]
    public class PipelinesVSMediatorBenchmark
    {
        private IMediator mediator;
        private int times = 100000;
        private ICommandDispatcher commandDispatcher;
    
        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddCommandHandler<BenchmarkCommand, BenchmarkCommandHandler>()
                .AddCommandPipelines(d => d
                    .SetupPreHandlers(v => v
                        .WithNext(typeof(BenchmarkCommandPreHandler))
                        .WithNext(typeof(BenchmarkCommandPreHandler2))
                        .WithNext(typeof(BenchmarkCommandPreHandler3))
                    ))
                .AddMediatR(typeof(PipelinesVSMediatorBenchmark))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(BenchmarkCommandPipelineBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(BenchmarkCommandPipelineBehavior2<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(BenchmarkCommandPipelineBehavior3<,>));

            var sp = services.BuildServiceProvider();

            mediator = sp.GetRequiredService<IMediator>();
            commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
        }
        [Benchmark]
        public async Task CQRS_With_Pipelines_1000_Invocations()
        {
            for (int i = 0; i < times; i++)
            {
                await commandDispatcher.Handle(new BenchmarkCommand(), default);
            }
        }
    
        [Benchmark]
        public async Task Mediator_With_PipelineBehaviour_1000_Invocations()
        {
            for (int i = 0; i < times; i++)
            {
                await mediator.Send(new BenchmarkRequest(), default);
            }
        }
    
        [Benchmark]
        public Task Mediator_SendingRequest()
        {
            return mediator.Send(new BenchmarkRequest());
        }
    
        [Benchmark]
        public Task CommandDispatcher_SendingCommand()
        {
            return commandDispatcher.Handle(new BenchmarkCommand(), default);
        }
    }

    public class BenchmarkCommand : ICommand {}

    public class BenchmarkRequest : IRequest<Result<Unit>> {}

    public class BenchmarkCommandPreHandler : ICommandPreHandlerPipe<BenchmarkCommand>
    {
        public Task<Result<Unit>> Handle(CommandPreHandlerPipelineContext<BenchmarkCommand> context, CommandPreHandlerPipelineDelegate<BenchmarkCommand> next,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandPipelineBehavior<TRequest, TResponse>: IPipelineBehavior<BenchmarkRequest, Result<Unit>>
    {
        public Task<Result<Unit>> Handle(BenchmarkRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<Unit>> next)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandPreHandler2 : ICommandPreHandlerPipe<BenchmarkCommand>
    {
        public Task<Result<Unit>> Handle(CommandPreHandlerPipelineContext<BenchmarkCommand> context, CommandPreHandlerPipelineDelegate<BenchmarkCommand> next,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandPipelineBehavior2<TRequest, TResponse>: IPipelineBehavior<BenchmarkRequest, Result<Unit>>
    {
        public Task<Result<Unit>> Handle(BenchmarkRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<Unit>> next)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandPreHandler3 : ICommandPreHandlerPipe<BenchmarkCommand>
    {
        public Task<Result<Unit>> Handle(CommandPreHandlerPipelineContext<BenchmarkCommand> context, CommandPreHandlerPipelineDelegate<BenchmarkCommand> next,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandPipelineBehavior3<TRequest, TResponse>: IPipelineBehavior<BenchmarkRequest, Result<Unit>>
    {
        public Task<Result<Unit>> Handle(BenchmarkRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<Unit>> next)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkCommandHandler : ICommandHandler<BenchmarkCommand>
    {
        public Task<Result<Unit>> Handle(BenchmarkCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    public class BenchmarkRequestHandler : IRequestHandler<BenchmarkRequest, Result<Unit>>
    {
        public Task<Result<Unit>> Handle(BenchmarkRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
}