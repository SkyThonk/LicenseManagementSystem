using System.Reflection;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces;
using Xunit;

namespace TenantService.Tests.Architecture;

public class CqrsHandlerArchitectureTests
{
    // Assembly containing application handlers (Identity.Application)
    private readonly Assembly _applicationAssembly;
    // Cached list of all discovered handler types in the assembly
    private readonly List<Type> _allHandlerTypes;

    public CqrsHandlerArchitectureTests()
    {
        // Use a well-known type from the application project to get its assembly
        _applicationAssembly = typeof(IUnitOfWork).Assembly; // Identity.Application assembly
        // Discover all potential Wolverine handlers on test construction
        _allHandlerTypes = GetAllWolverineHandlers();
    }

    /// <summary>
    /// Get all types that have a Handle method (Wolverine handlers)
    /// </summary>
    // Use reflection to find concrete classes that expose a public instance Handle method.
    // We consider handlers to have a Handle method with at least one parameter and a generic return type.
    private List<Type> GetAllWolverineHandlers()
    {
        return _applicationAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Any(m => m.Name == "Handle" && 
                          m.GetParameters().Length >= 1 && 
                          m.ReturnType.IsGenericType))
            .ToList();
    }

    [Fact]
    public void AllWolverineHandlers_Should_ImplementEither_IQueryHandler_Or_ICommandHandler()
    {
        var failures = new List<string>();

        // Ensure each discovered handler implements either IQueryHandler<,> or ICommandHandler<,>
        foreach (var handlerType in _allHandlerTypes)
        {
            var interfaces = handlerType.GetInterfaces();
            
            var implementsQueryHandler = interfaces.Any(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                
            var implementsCommandHandler = interfaces.Any(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

            if (!implementsQueryHandler && !implementsCommandHandler)
            {
                // Record failure if neither interface is implemented
                failures.Add($"{handlerType.Name} does not implement IQueryHandler<,> or ICommandHandler<,>");
            }
        }

        // Assert no failures were collected
        Assert.Empty(failures);
    }

    [Fact]
    public void QueryHandlers_Should_Not_Have_IUnitOfWork_Dependency()
    {
        var failures = new List<string>();

        // Filter only query handlers by interface
        var queryHandlers = _allHandlerTypes
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                          i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .ToList();

        foreach (var queryHandler in queryHandlers)
        {
            // Check constructor parameters for IUnitOfWork dependency
            var constructors = queryHandler.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var hasUnitOfWork = parameters.Any(p => p.ParameterType == typeof(IUnitOfWork));
                
                if (hasUnitOfWork)
                {
                    // Query handlers should not depend on Unit of Work directly
                    failures.Add($"{queryHandler.Name} is a query handler but has IUnitOfWork dependency");
                }
            }

            // Also check private fields to ensure IUnitOfWork isn't stored internally
            var fields = queryHandler.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var hasUnitOfWorkField = fields.Any(f => f.FieldType == typeof(IUnitOfWork));
            
            if (hasUnitOfWorkField)
            {
                failures.Add($"{queryHandler.Name} is a query handler but has IUnitOfWork field");
            }
        }

        // Expect no query handlers to reference IUnitOfWork
        Assert.Empty(failures);
    }

    [Fact]
    public void CommandHandlers_Should_Have_IUnitOfWork_Dependency()
    {
        var failures = new List<string>();

        // Filter only command handlers by interface
        var commandHandlers = _allHandlerTypes
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                          i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            .ToList();

        foreach (var commandHandler in commandHandlers)
        {
            // Check constructors first for IUnitOfWork dependency
            var constructors = commandHandler.GetConstructors();
            var hasUnitOfWork = false;

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                hasUnitOfWork = parameters.Any(p => p.ParameterType == typeof(IUnitOfWork));
                
                if (hasUnitOfWork)
                    break;
            }

            // If not found in constructors, check private fields as fallback
            if (!hasUnitOfWork)
            {
                var fields = commandHandler.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                hasUnitOfWork = fields.Any(f => f.FieldType == typeof(IUnitOfWork));
            }

            if (!hasUnitOfWork)
            {
                // Command handlers are expected to coordinate persistence via Unit of Work
                failures.Add($"{commandHandler.Name} is a command handler but does NOT have IUnitOfWork dependency");
            }
        }

        // Assert all command handlers have Unit of Work
        Assert.Empty(failures);
    }

    [Fact]
    public void EventHandlers_Should_Follow_Naming_Convention()
    {
        var failures = new List<string>();

        // Event handlers typically have names ending with "EventHandler"
        var eventHandlers = _allHandlerTypes
            .Where(t => t.Name.EndsWith("EventHandler"))
            .ToList();

        foreach (var eventHandler in eventHandlers)
        {
            // Event handlers should handle domain events (first parameter ends with "Event")
            var handleMethods = eventHandler.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "Handle")
                .ToList();

            foreach (var handleMethod in handleMethods)
            {
                var parameters = handleMethod.GetParameters();
                if (parameters.Length > 0)
                {
                    var firstParam = parameters[0];
                    if (!firstParam.ParameterType.Name.EndsWith("Event"))
                    {
                        // Record when first parameter type doesn't follow event naming convention
                        failures.Add($"{eventHandler.Name}.Handle() first parameter {firstParam.ParameterType.Name} should end with 'Event'");
                    }
                }
            }
        }

        // Expect event handlers to follow naming conventions
        Assert.Empty(failures);
    }

    [Fact]
    public void Handlers_Should_Be_In_Correct_Namespace()
    {
        var failures = new List<string>();

        foreach (var handlerType in _allHandlerTypes)
        {
            var @namespace = handlerType.Namespace ?? string.Empty;
            
            // Query handlers should live under a .Query namespace
            if (handlerType.GetInterfaces().Any(i => i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            {
                if (!@namespace.Contains(".Query"))
                {
                    failures.Add($"{handlerType.Name} is a query handler but not in a .Query namespace");
                }
            }
            
            // Command handlers should live under a .Command namespace
            if (handlerType.GetInterfaces().Any(i => i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            {
                if (!@namespace.Contains(".Command"))
                {
                    failures.Add($"{handlerType.Name} is a command handler but not in a .Command namespace");
                }
            }
            
            // Event handlers should live under an .Event namespace
            if (handlerType.Name.EndsWith("EventHandler"))
            {
                if (!@namespace.Contains(".Event"))
                {
                    failures.Add($"{handlerType.Name} is an event handler but not in a .Event namespace");
                }
            }
        }

        // Assert that every handler is placed in expected namespace
        Assert.Empty(failures);
    }

    [Fact]
    public void Handlers_Should_Return_Result_Type()
    {
        var failures = new List<string>();

        foreach (var handlerType in _allHandlerTypes)
        {
            // Inspect all public instance Handle methods on the handler
            var handleMethods = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "Handle")
                .ToList();

            foreach (var handleMethod in handleMethods)
            {
                var returnType = handleMethod.ReturnType;
                
                // We expect command/query handlers to return Task<Result<T>> (i.e., Task of a Result type)
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var taskType = returnType.GetGenericArguments()[0];
                    
                    // Skip event handlers which often return plain Task or different shapes
                    if (handlerType.Name.EndsWith("EventHandler"))
                        continue;
                    
                    // For command and query handlers, ensure the inner Task<> type is a Result<T> variant
                    if (!taskType.Name.StartsWith("Result"))
                    {
                        failures.Add($"{handlerType.Name}.Handle() should return Task<Result<TResponse>> but returns {returnType.Name}");
                    }
                }
            }
        }

        // Verify all handlers conform to Result<T> return convention
        Assert.Empty(failures);
    }
}

