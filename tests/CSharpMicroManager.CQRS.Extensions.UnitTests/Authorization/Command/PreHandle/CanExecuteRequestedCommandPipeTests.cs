using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpMicroManager.CQRS.Abstractions.Attributes.Command;
using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Extensions.Authorization.Command.PreHandle;
using CSharpMicroManager.Functional.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CSharpMicroManager.CQRS.Extensions.UnitTests.Authorization.Command.PreHandle;

[TestFixture]
public class CanExecuteRequestedCommandPipeTests 
{
    [Test]
    public async Task ShouldReturnError_WhenRequiredPermissions_AreNotMetOnClassLevel()
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddTransient<IUserContext, FakeUserContext>()
            .AddCommandHandler<TestCommand, TestCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .UseCanExecuteRequestedCommandPipe()));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Handle(new TestCommand(), default);

        result.IsError.Should().BeTrue();
    }
    
    [Test]
    public async Task ShouldReturnError_WhenRequiredPermissions_AreNotMetOnPropertyLevel()
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddTransient<IUserContext, FakeUserContext>()
            .AddCommandHandler<TestPropertyCommand, TestPropertyCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .UseCanExecuteRequestedCommandPipe()));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Handle(new TestPropertyCommand(), default);

        result.IsError.Should().BeTrue();
    }
    
    [Test]
    public async Task ShouldExecuteCommand_WhenRequiredPermissions_AreMetOnClassLevel()
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddTransient<IUserContext>(sp => new FakeUserContext(new List<Guid>
            {
                new TestCommandPermission().Id
            }))
            .AddCommandHandler<TestCommand, TestCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .UseCanExecuteRequestedCommandPipe()));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Handle(new TestCommand(), default);

        result.IsError.Should().BeFalse();
    }

    [Test]
    public async Task ShouldExecuteCommand_WhenRequiredPermissions_AreMetOnPropertyLevel()
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddTransient<IUserContext>(sp => new FakeUserContext(new List<Guid>
            {
                new TestPropertyPermission().Id
            }))
            .AddCommandHandler<TestPropertyCommand, TestPropertyCommandHandler>()
            .AddCommandPipelines(d => d
                .SetupPreHandlers(v => v
                    .UseCanExecuteRequestedCommandPipe()));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Handle(new TestPropertyCommand(), default);

        result.IsError.Should().BeFalse();
    }
    
    record TestCommandPermission : CommandPermission
    {
        public override Guid Id => Guid.Parse("161ad478-5f82-411f-80c1-138b0a4e26b1");
    }
    
    [RequirePermission<TestCommandPermission>]
    class TestCommand : ICommand
    {
        
    }
    
    class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<Result<Unit>> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }
    
    record TestPropertyPermission : CommandPermission
    {
        public override Guid Id => Guid.Parse("234ad478-5f82-411f-80c1-111b0a4e26b1");
    }
    
    class TestPropertyCommand : ICommand
    {
        [RequirePermission<TestPropertyPermission>]
        public string Something { get; }
    }
    
    class TestPropertyCommandHandler : ICommandHandler<TestPropertyCommand>
    {
        public Task<Result<Unit>> Handle(TestPropertyCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result<Unit>(Unit.Value));
        }
    }

    class FakeUserContext : IUserContext
    {
        private readonly IReadOnlyCollection<Guid>? _permissions;

        public FakeUserContext(IReadOnlyCollection<Guid>? permissions = null)
        {
            _permissions = permissions;
        }
        public IReadOnlyCollection<Guid> GetAllUserPermissionIds()
        {
            return _permissions ?? new List<Guid>();
        }
    }
}