using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.CQRS.Attributes.Command;
using CSharpMicroManager.CQRS.Decorators.Command;
using CSharpMicroManager.CQRS.Dispatching.Command;
using CSharpMicroManager.CQRS.Dispatching.Query;
using CSharpMicroManager.CQRS.Pipelines.Command;
using CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;
using CSharpMicroManager.CQRS.Pipelines.Query;
using CSharpMicroManager.CQRS.Pipelines.Query.Descriptors;

[assembly: InternalsVisibleTo("CSharpMicroManager.CQRS.UnitTests")]
namespace CSharpMicroManager.CQRS.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandler<TCommand, TCommandHandler>(
        this IServiceCollection services)
        where TCommand : ICommand
        where TCommandHandler : class, ICommandHandler<TCommand>
    {
         services.AddTransient<TCommandHandler>()
            .AddTransient<ICommandHandler<TCommand>>(sp => sp.GetRequiredService<TCommandHandler>());
         return services;
    }
    
    public static IServiceCollection AddQueryHandler<TQuery, TResult, TQueryHandler>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
        where TQueryHandler : class, IQueryHandler<TQuery, TResult>
    {
        return services.AddTransient<TQueryHandler>()
            .AddTransient<IQueryHandler<TQuery, TResult>>(sp => sp.GetRequiredService<TQueryHandler>());
    }

    public static IServiceCollection AddCommandPipelines(
        this IServiceCollection services,
        Func<CommandPipelinesDescriptor, CommandPipelinesDescriptor>? descriptorFunc = null)
    {
        var descriptor = descriptorFunc?.Invoke(new CommandPipelinesDescriptor().WithServices(services)) 
                         ?? new CommandPipelinesDescriptor().WithServices(services);
        
        descriptor.PreHandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });
        
        descriptor.HandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });
        
        descriptor.PostHandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });

        return services
            .AddTransient<ICommandDispatcher, CommandDispatcher>()
            .AddTransient(typeof(ICommandPreHandlerPipelineBuilder<>), typeof(CommandPreHandlerPipelineBuilder<>))
            .AddTransient(typeof(ICommandHandlerPipelineBuilder<>), typeof(CommandHandlerPipelineBuilder<>))
            .AddTransient(typeof(ICommandPostHandlerPipelineBuilder<>), typeof(CommandPostHandlerPipelineBuilder<>))
            .AddTransient(typeof(CommandPipelineBuilderFactory<>));
    }
    
    public static IServiceCollection AddQueryPipelines(
        this IServiceCollection services,
        Func<QueryPipelinesDescriptor, QueryPipelinesDescriptor>? descriptorFunc = null)
    {
        var descriptor = descriptorFunc?.Invoke(new QueryPipelinesDescriptor(services)) ?? new(services);

        descriptor.PreHandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });
        
        descriptor.HandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });

        descriptor.PostHandlerPipesDescriptor.SortedPipes.ToList().ForEach(kv =>
        {
            services.AddTransient(kv.Value.ServiceType, kv.Value.ImplementationType!);
        });

        return services
            .AddTransient<IQueryDispatcher, QueryDispatcher>()
            .AddTransient<QueryPipelineBuilderFactory>();
    }
    
    
    #region obsolete
    [Obsolete("Please use registering single command separately", false)]
    public static IServiceCollection AddCommandHandlers(
        this IServiceCollection services,
        Assembly from,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        return services.Scan(scan => scan
            .FromAssemblies(from)
            .AddClasses(classes =>
                classes.AssignableTo(typeof(ICommandHandler<>))
                    .Where(c => !c.IsAbstract && !c.IsGenericTypeDefinition))
            .AsSelfWithInterfaces()
            .WithLifetime(lifetime)
        );
    }

    [Obsolete("Please start using Pipeline approach")]
    public static IServiceCollection DecorateCommandHandlersWith(
        this IServiceCollection services,
        Type decoratorType)
    {
        if (decoratorType.BaseType == null ||
            !decoratorType.BaseType.IsGenericType ||
            !(decoratorType.BaseType.GetGenericTypeDefinition() == typeof(CommandHandlerDecorator<>)))
        {
            throw new InvalidOperationException(
                "Cannot register decorator when does not inherit from CommandHandlerDecorator<TCommand,TResponse>");
        }

        services.Where(sd =>
            sd.ServiceType.IsGenericType && sd.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) &&
            sd.ServiceType.IsInterface).Select(sd => sd.ServiceType).ToList().ForEach(commandHandlerType =>
        {
            services.Decorate(
                commandHandlerType,
                decoratorType.MakeGenericType(commandHandlerType.GetGenericArguments().Single()
                )
            );
        });

        return services;
    }


    /// <summary>
    /// Registers automatically defined decorators in the assembly <paramref name="from"/>.
    /// Tries to register then in correct order of invocation by using specific <see cref="CommandHandlerAttribute"/>
    /// <list type="number">
    /// <item>Everything is wrapped in ErrorHandlerDecorator decorated with <see cref="ErrorHandlerDecoratorAttribute"/></item>
    /// <item>Then wrapped in LoggerDecorator decorated with <see cref="LoggerDecoratorAttribute"/></item>
    /// <item>Then wrapped in UnitOfWorkDecorator decorated with <see cref="UnitOfWorkDecoratorAttribute"/></item>
    /// <item>Then wrapped in DomainEventsDispatcherDecorator decorated with <see cref="DomainEventsHandlerDecoratorAttribute"/></item>
    /// </list>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    [Obsolete("Please start using Pipeline approach")]
    public static IServiceCollection AddCommandHandlerDecorators(this IServiceCollection services, Assembly from)
    {
        var decoratorTypes = @from.GetTypes().Where(t => !t.IsAbstract &&
                                                         !t.IsInterface &&
                                                         t.BaseType != null &&
                                                         t.BaseType.IsGenericType &&
                                                         t.BaseType.GetGenericTypeDefinition() ==
                                                         typeof(CommandHandlerDecorator<>)).ToList();

        if (decoratorTypes.Any(dt =>
                dt.GetCustomAttributes().All(ca => ca.GetType().BaseType != typeof(CommandHandlerAttribute))))
        {
            throw new InvalidOperationException(
                $@"Cannot add implicitly CommandHandler decorators in correct order, when do not have {nameof(CommandHandlerAttribute)}.
                                                      Please use one from namespace: {typeof(CommandHandlerAttribute).Namespace}");
        }

        var orderedDecorators = decoratorTypes
            .OrderBy(dt => dt.GetCustomAttributes().Any(ca => ca is ErrorHandlerDecoratorAttribute))
            .ThenBy(dt => dt.GetCustomAttributes().Any(ca => ca is LoggerDecoratorAttribute))
            .ThenBy(dt => dt.GetCustomAttributes().Any(ca => ca is UnitOfWorkDecoratorAttribute)).ThenBy(dt =>
                dt.GetCustomAttributes().Any(ca => ca is DomainEventsHandlerDecoratorAttribute));

        foreach (var decorator in orderedDecorators)
        {
            DecorateCommandHandlersWith(services, decorator);
        }

        return services;
    }
    
    [Obsolete("Please use registering single query separately", false)]
    public static IServiceCollection AddQueryHandlers(
        this IServiceCollection services,
        Assembly from,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        return services.Scan(scan => scan
            .FromAssemblies(from)
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IQueryHandler<,>))
                    .Where(c => !c.IsAbstract && !c.IsGenericTypeDefinition))
            .AsSelfWithInterfaces()
            .WithLifetime(lifetime)
        );
    }

    #endregion
}