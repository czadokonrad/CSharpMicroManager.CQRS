using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.CQRS.Extensions;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Attributes.Command;
using CSharpMicroManager.CQRS.Decorators.Command;

namespace CSharpMicroManager.CQRS.UnitTests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    public record TestCommand : ICommand { }

    private class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<Result<CSharpMicroManager.Functional.Core.Unit>> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult<Result<Unit>>(CSharpMicroManager.Functional.Core.Unit.Value);
        }
    }

    private record TestQuery : IQuery<bool> { }

    private class TestQueryHandler : IQueryHandler<TestQuery, bool>
    {
        public Task<Result<Option<bool>>> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult<Result<Option<bool>>>(new Option<bool>(true));
        }
    }

    [Test]
    public void When_CommandHandler_Registered_Then_CanBeResolved_As_Concrete_Type()
    {
        var services = new ServiceCollection().
            AddCommandHandler<TestCommand, TestCommandHandler>();

        var sp = services.BuildServiceProvider();

        var commandHandler = sp.GetRequiredService<TestCommandHandler>();

        Assert.IsNotNull(commandHandler);
    }

    [Test]
    public void When_CommandHandler_Registered_Then_CanBeResolved_As_Interface()
    {
        var services = new ServiceCollection().
            AddCommandHandler<TestCommand, TestCommandHandler>();

        var sp = services.BuildServiceProvider();

        var commandHandler = sp.GetRequiredService<ICommandHandler<TestCommand>>();

        Assert.IsNotNull(commandHandler);
    }


    [Test]
    public void When_CommandHandler_Registered_Using_Scrutor_Then_CanBeResolved_As_Concrete_Type()
    {
        var services = new ServiceCollection().
            AddCommandHandlers(typeof(TestCommandHandler).Assembly);

        var sp = services.BuildServiceProvider();

        var commandHandler = sp.GetRequiredService<TestCommandHandler>();

        Assert.IsNotNull(commandHandler);
    }

    [Test]
    public void When_CommandHandler_Registered_Using_Scrutor_Then_CanBeResolved_As_Interface()
    {
        var services = new ServiceCollection().
            AddCommandHandlers(typeof(TestCommandHandler).Assembly);

        var sp = services.BuildServiceProvider();

        var commandHandler = sp.GetRequiredService<ICommandHandler<TestCommand>>();

        Assert.IsNotNull(commandHandler);
    }

    [Test]
    public void When_QueryHandler_Registered_Then_CanBeResolved_As_Concrete_Type()
    {
        var services = new ServiceCollection().
            AddQueryHandler<TestQuery, bool, TestQueryHandler>();

        var sp = services.BuildServiceProvider();

        var queryHandler = sp.GetRequiredService<TestQueryHandler>();

        Assert.IsNotNull(queryHandler);
    }

    [Test]
    public void When_QueryHandler_Registered_Then_CanBeResolved_As_Interface()
    {
        var services = new ServiceCollection().
            AddQueryHandler<TestQuery, bool, TestQueryHandler>();

        var sp = services.BuildServiceProvider();

        var queryHandler = sp.GetRequiredService<IQueryHandler<TestQuery, bool>>();

        Assert.IsNotNull(queryHandler);
    }


    [Test]
    public void When_QueryHandler_Registered_Using_Scrutor_Then_CanBeResolved_As_Concrete_Type()
    {
        var services = new ServiceCollection().
            AddQueryHandlers(typeof(TestQueryHandler).Assembly);

        var sp = services.BuildServiceProvider();

        var queryHandler = sp.GetRequiredService<TestQueryHandler>();

        Assert.IsNotNull(queryHandler);
    }

    [Test]
    public void When_QueryHandler_Registered_Using_Scrutor_Then_CanBeResolved_As_Interface()
    {
        var services = new ServiceCollection().
            AddQueryHandlers(typeof(TestQueryHandler).Assembly);

        var sp = services.BuildServiceProvider();

        var queryHandler = sp.GetRequiredService<IQueryHandler<TestQuery, bool>>();

        Assert.IsNotNull(queryHandler);
    }

    [Test]
    public async Task When_CommandHandlers_Registered_And_Decorator_With_CommandHandlerAttribute_Applied_Then_Command_Handlers_Should_Be_Wrapped_In_Decorator()
    {
        var services = new ServiceCollection().
            AddLogging().
            AddTransient(typeof(MockDomainEventsDispatcher)).
            AddTransient(typeof(MockUnitOfWork)).
            AddCommandHandlers(typeof(TestCommandHandler).Assembly).
            DecorateCommandHandlersWith(typeof(DomainEventsHandlerCommandHandlerDecorator<>)).
            DecorateCommandHandlersWith(typeof(UnitOfWorkCommandHandlerDecorator<>));

        var sp = services.BuildServiceProvider();
        var commandHandler = sp.GetRequiredService<ICommandHandler<TestCommand>>();

        await commandHandler.Handle(new TestCommand(), default);

        Assert.IsTrue(MockDomainEventsDispatcher.InvokeCount == 1);
        Assert.IsTrue(MockUnitOfWork.InvokeCount == 1);
    }


    [Test]
    public async Task When_CommandHandlers_Registered_Using_AddCommandHandlerDecoratorsAnd_Then_Command_Handlers_Should_Be_Wrapped_In_Decorators_InCorrectOrder()
    {
        var services = new ServiceCollection().
            AddLogging().
            AddTransient(typeof(MockDomainEventsDispatcher)).
            AddTransient(typeof(MockUnitOfWork)).
            AddCommandHandlers(typeof(TestCommandHandler).Assembly).
            AddCommandHandlerDecorators(typeof(DomainEventsHandlerCommandHandlerDecorator<>).Assembly);

        var sp = services.BuildServiceProvider();
        var commandHandler = sp.GetRequiredService<ICommandHandler<TestCommand>>();

        await commandHandler.Handle(new TestCommand(), default);

        Assert.IsTrue(MockDomainEventsDispatcher.InvokeCount == 1);
        Assert.IsTrue(MockUnitOfWork.InvokeCount == 1);
    }
    private sealed class MockDomainEventsDispatcher
    {
        public static int InvokeCount;

        public MockDomainEventsDispatcher()
        {
            InvokeCount = 0;
        }

        public Task Dispatch(CancellationToken cancellationToken)
        {
            InvokeCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class MockUnitOfWork
    {
        public static int InvokeCount;

        public MockUnitOfWork()
        {
            InvokeCount = 0;
        }
        public Task Commit(CancellationToken cancellationToken)
        {
            InvokeCount++;
            return Task.CompletedTask;
        }
    }

    [DomainEventsHandlerDecorator]
    private sealed class DomainEventsHandlerCommandHandlerDecorator<TCommand> : CommandHandlerDecorator<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly MockDomainEventsDispatcher _domainEventsDispatcher;
        private readonly ILogger<DomainEventsHandlerCommandHandlerDecorator<TCommand>> _logger;

        public DomainEventsHandlerCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated,
            MockDomainEventsDispatcher domainEventsDispatcher,
            ILogger<DomainEventsHandlerCommandHandlerDecorator<TCommand>> logger)
        {
            _decorated = decorated;
            _domainEventsDispatcher = domainEventsDispatcher;
            _logger = logger;
        }

        public override async Task<Result<CSharpMicroManager.Functional.Core.Unit>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var result = await _decorated.Handle(command, cancellationToken);

            await _domainEventsDispatcher.Dispatch(cancellationToken);


            return result;
        }
    }

    [UnitOfWorkDecorator]
    private sealed class UnitOfWorkCommandHandlerDecorator<TCommand> : CommandHandlerDecorator<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly MockUnitOfWork _unitOfWork;

        public UnitOfWorkCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated,
            MockUnitOfWork unitOfWork)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<CSharpMicroManager.Functional.Core.Unit>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var result = await _decorated.Handle(command, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);
            return CSharpMicroManager.Functional.Core.Unit.Value;

        }
    }
}