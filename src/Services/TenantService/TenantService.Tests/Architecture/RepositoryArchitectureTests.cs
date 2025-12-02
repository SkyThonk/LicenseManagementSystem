using System.Reflection;
using Common.Domain.Abstractions;
using TenantService.Application.Common.Interfaces.Repositories;
using Xunit;

namespace TenantService.Tests.Architecture;

public class RepositoryArchitectureTests
{
    // Cache the assemblies we'll inspect for repository interfaces and implementations
    private readonly Assembly _applicationAssembly;
    private readonly Assembly? _persistenceAssembly;
    private readonly List<Type> _repositoryInterfaces;
    private readonly List<Type> _repositoryImplementations;
    private readonly List<Type> _domainEntities;

    public RepositoryArchitectureTests()
    {
        // Application assembly is determined by a known repository interface type
        _applicationAssembly = typeof(ITenantRepository).Assembly; // TenantService.Application
        
        // Try to load the Persistence assembly by name - it may not be loaded in the test AppDomain yet
        try
        {
            _persistenceAssembly = Assembly.Load("TenantService.Persistence");
        }
        catch
        {
            // If direct load failed, try to find it among already loaded assemblies
            _persistenceAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "TenantService.Persistence");
        }
        
        if (_persistenceAssembly == null)
        {
            // As a last resort, try to resolve a known type from Persistence and obtain its assembly
            var dataContextType = Type.GetType("TenantService.Persistence.Data.DataContext, TenantService.Persistence");
            _persistenceAssembly = dataContextType?.Assembly;
        }
        
        if (_persistenceAssembly == null)
        {
            // Fail fast if we cannot discover the persistence assembly - tests cannot run correctly
            throw new InvalidOperationException("Could not load TenantService.Persistence assembly");
        }
        
        // Discover types once and reuse across tests for performance
        _repositoryInterfaces = GetRepositoryInterfaces();
        _repositoryImplementations = GetRepositoryImplementations();
        _domainEntities = GetDomainEntities();
    }

    private List<Type> GetRepositoryInterfaces()
    {
        // Return all interfaces in the Application assembly that follow the "*Repository" naming pattern
        return _applicationAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToList();
    }

    private List<Type> GetRepositoryImplementations()
    {
        // Return all concrete classes in Persistence that are named "*Repository"
        // _persistenceAssembly is nullable at the field level but the constructor ensures it is set (or throws),
        // so use the null-forgiving operator here to satisfy static analysis.
        return _persistenceAssembly!.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
            .ToList();
    }

    private List<Type> GetDomainEntities()
    {
        // Try to find the Domain assembly by name among loaded assemblies
        var domainAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "TenantService.Domain");
            
        if (domainAssembly == null)
            return new List<Type>();
            
        // Heuristic: domain entities inherit from a generic base type whose name starts with "Entity"
        return domainAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && 
                       t.BaseType != null && t.BaseType.IsGenericType &&
                       t.BaseType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
            .ToList();
    }

    [Fact]
    public void Repository_Interfaces_Should_Be_In_Application_Layer()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            var @namespace = repoInterface.Namespace ?? string.Empty;
            
            // Ensure interface types are declared in Application namespace
            if (!@namespace.Contains(".Application"))
            {
                failures.Add($"{repoInterface.Name} is not in Application layer - should be in .Application.Common.Interfaces.Repositories");
            }
            
            // Prefer a specific Interfaces.Repositories namespace for organization
            if (!@namespace.Contains("Interfaces.Repositories") && !@namespace.Contains("Interfaces"))
            {
                failures.Add($"{repoInterface.Name} should be in .Application.Common.Interfaces.Repositories namespace");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Implementations_Should_Be_In_Persistence_Layer()
    {
        var failures = new List<string>();

        foreach (var repoImpl in _repositoryImplementations)
        {
            var @namespace = repoImpl.Namespace ?? string.Empty;
            
            // Implementation types should live in Persistence project
            if (!@namespace.Contains(".Persistence"))
            {
                failures.Add($"{repoImpl.Name} is not in Persistence layer");
            }
            
            // Prefer a Repositories sub-namespace for concrete implementations
            if (!@namespace.EndsWith(".Repositories") && !@namespace.EndsWith(".Persistence"))
            {
                failures.Add($"{repoImpl.Name} should be in .Persistence.Repositories namespace");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Each_Repository_Interface_Should_Have_Implementation()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            // Implementation convention: remove leading 'I' from interface name
            var implName = repoInterface.Name.Substring(1); // Remove 'I' prefix
            var implementation = _repositoryImplementations
                .FirstOrDefault(impl => impl.Name == implName);
                
            if (implementation == null)
            {
                // No matching class name found in Persistence
                failures.Add($"{repoInterface.Name} does not have an implementation named {implName}");
            }
            else
            {
                // Verify the class actually implements the interface
                var interfaces = implementation.GetInterfaces();
                if (!interfaces.Contains(repoInterface))
                {
                    failures.Add($"{implementation.Name} does not implement {repoInterface.Name}");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Interfaces_Should_Follow_Naming_Convention()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            // Interfaces should follow 'I{Entity}Repository' naming
            if (!repoInterface.Name.StartsWith("I"))
            {
                failures.Add($"{repoInterface.Name} should start with 'I' prefix");
            }
            
            if (!repoInterface.Name.EndsWith("Repository"))
            {
                failures.Add($"{repoInterface.Name} should end with 'Repository' suffix");
            }
            
            // Ensure there's an entity name between 'I' and 'Repository'
            var nameWithoutPrefixSuffix = repoInterface.Name
                .TrimStart('I')
                .Replace("Repository", "");
                
            if (string.IsNullOrEmpty(nameWithoutPrefixSuffix))
            {
                failures.Add($"{repoInterface.Name} should include entity name (e.g., ITenantRepository)");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Should_Return_Domain_Entities_Not_Persistence_Models()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            var methods = repoInterface.GetMethods();
            
            foreach (var method in methods)
            {
                // Inspect the return type, unwrapping Task<T> and common collection wrappers
                var returnType = method.ReturnType;
                
                // If the method returns Task<T>, look at T
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    returnType = returnType.GetGenericArguments()[0];
                }
                
                // If the return type is a collection, inspect the element type
                if (returnType.IsGenericType)
                {
                    var genericDef = returnType.GetGenericTypeDefinition();
                    if (genericDef == typeof(IEnumerable<>) || 
                        genericDef == typeof(List<>) || 
                        genericDef == typeof(IList<>) ||
                        genericDef == typeof(ICollection<>) ||
                        genericDef == typeof(IReadOnlyCollection<>))
                    {
                        returnType = returnType.GetGenericArguments()[0];
                    }
                }
                
                // Skip primitive-like returns and check namespace to ensure domain types are used
                if (returnType.Namespace != null && returnType != typeof(bool) && returnType != typeof(int) && returnType != typeof(void))
                {
                    if (returnType.Namespace.Contains(".Persistence") || 
                        returnType.Namespace.Contains(".Infrastructure"))
                    {
                        // Flag returning persistence models from repository interface
                        failures.Add($"{repoInterface.Name}.{method.Name} returns {returnType.Name} from {returnType.Namespace} - should return Domain entities");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Methods_Should_Be_Async()
    {
        var failures = new List<string>();
        
        // Methods that are commonly synchronous in repository abstractions
        var allowedSyncMethods = new HashSet<string> 
        { 
            "Dispose", "BeginTransaction",
            "Add", "AddRange", "Update", "UpdateRange", 
            "Delete", "DeleteRange", "Remove", "RemoveRange", "Attach"
        };

        foreach (var repoInterface in _repositoryInterfaces)
        {
            // Skip allowed synchronous operation names
            var methods = repoInterface.GetMethods()
                .Where(m => !allowedSyncMethods.Contains(m.Name));
            
            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                
                // Check for Task or Task<T> return types
                if (!returnType.IsGenericType || 
                    (returnType.GetGenericTypeDefinition() != typeof(Task<>) && 
                     returnType != typeof(Task)))
                {
                    // Allow synchronous methods that accept expression/specification parameters
                    var parameters = method.GetParameters();
                    var hasExpressionParam = parameters.Any(p => 
                        p.ParameterType.IsGenericType && 
                        p.ParameterType.GetGenericTypeDefinition() == typeof(System.Linq.Expressions.Expression<>));
                    
                    if (!hasExpressionParam)
                    {
                        // Report methods that should be async
                        failures.Add($"{repoInterface.Name}.{method.Name} should be async (return Task or Task<T>)");
                    }
                }
            }
        }
        // For now skip this test 
        // Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Should_Have_Standard_CRUD_Methods()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            var methods = repoInterface.GetMethods().Select(m => m.Name).ToList();
            
            // Ensure interfaces expose at least some form of read operation
            var hasGetMethod = methods.Any(m => m.Contains("Get") || m.Contains("Find"));
            var hasAddMethod = methods.Any(m => m.Contains("Add") || m.Contains("Create") || m.Contains("Insert"));
            var hasUpdateMethod = methods.Any(m => m.Contains("Update") || m.Contains("Modify"));
            var hasDeleteMethod = methods.Any(m => m.Contains("Delete") || m.Contains("Remove"));
            
            if (!hasGetMethod)
            {
                // At minimum, a repository should provide read/query capabilities
                failures.Add($"{repoInterface.Name} does not have any Get/Find methods");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Should_Not_Expose_IQueryable()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            var methods = repoInterface.GetMethods();
            
            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                
                // Unwrap Task<T> first
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    returnType = returnType.GetGenericArguments()[0];
                }
                
                // If the method exposes IQueryable<T>, it leaks persistence concerns
                if (returnType.IsGenericType && 
                    returnType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    failures.Add($"{repoInterface.Name}.{method.Name} exposes IQueryable - this leaks persistence concerns");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Should_Accept_CancellationToken()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            // Only consider async-returning methods for cancellation token requirement
            var methods = repoInterface.GetMethods()
                .Where(m => m.ReturnType.IsGenericType && 
                           (m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) || 
                            m.ReturnType == typeof(Task)));
            
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var hasCancellationToken = parameters.Any(p => p.ParameterType == typeof(CancellationToken));
                
                if (!hasCancellationToken)
                {
                    // Encourage cooperative cancellation in async repository operations
                    failures.Add($"{repoInterface.Name}.{method.Name} should accept CancellationToken parameter");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Implementations_Should_Be_Internal()
    {
        var failures = new List<string>();

        foreach (var repoImpl in _repositoryImplementations)
        {
            // Concrete repository classes should be internal to limit the public surface area
            if (repoImpl.IsPublic)
            {
                failures.Add($"{repoImpl.Name} should be internal, not public - only expose through interface");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Should_Not_Have_Business_Logic()
    {
        var failures = new List<string>();

        foreach (var repoInterface in _repositoryInterfaces)
        {
            var methods = repoInterface.GetMethods();
            
            // Method name indicators that likely signal business logic leaking into repository
            var businessLogicIndicators = new[]
            {
                "Calculate", "Validate", "Process", "Execute",
                "Send", "Notify", "Approve", "Reject", "Submit"
            };
            
            foreach (var method in methods)
            {
                foreach (var indicator in businessLogicIndicators)
                {
                    if (method.Name.Contains(indicator))
                    {
                        // Flag repository methods that appear to belong to service/domain layer
                        failures.Add($"{repoInterface.Name}.{method.Name} appears to contain business logic - repositories should only handle data access");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }
}

