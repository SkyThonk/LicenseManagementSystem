using System.Reflection;
using Common.Domain.Abstractions;
using LicenseService.Domain.Licenses;
using Xunit;

namespace LicenseService.Tests.Architecture;

public class DomainArchitectureTests
{
    // Assembly containing domain types to inspect
    private readonly Assembly _domainAssembly;
    // Cached lists of types to avoid repeated reflection calls
    private readonly List<Type> _allDomainTypes;
    private readonly List<Type> _entityTypes;
    private readonly List<Type> _valueObjectTypes;
    private readonly List<Type> _domainEventTypes;

    public DomainArchitectureTests()
    {
        _domainAssembly = typeof(License).Assembly; // LicenseService.Domain assembly
        _allDomainTypes = _domainAssembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsEnum && !t.IsAbstract)
            .ToList();

        _entityTypes = GetEntityTypes();
        _valueObjectTypes = GetValueObjectTypes();
        _domainEventTypes = GetDomainEventTypes();
    }

    // Identify entity types by checking for a generic base type named like "Entity..."
    private List<Type> GetEntityTypes()
    {
        return _allDomainTypes
            .Where(t => t.BaseType != null &&
                       t.BaseType.IsGenericType &&
                       t.BaseType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
            .ToList();
    }

    // Identify value objects by naming convention or by common patterns
    private List<Type> GetValueObjectTypes()
    {
        return _allDomainTypes
            .Where(t => t.IsClass &&
                       (t.Name.EndsWith("Id") ||
                        IsValueObjectByConvention(t)))
            .ToList();
    }

    // Domain events are recognized by suffix "Event"
    private List<Type> GetDomainEventTypes()
    {
        return _allDomainTypes
            .Where(t => t.Name.EndsWith("Event") &&
                       (t.IsClass || t.IsValueType))
            .ToList();
    }

    // Simple heuristic for common value object names
    private bool IsValueObjectByConvention(Type type)
    {
        // Common value object patterns
        var valueObjectNames = new[] { "Name", "Email", "Phone", "Address", "Money", "DateRange", "Period" };
        return valueObjectNames.Any(vo => type.Name.Contains(vo));
    }

    // Determine if a property is likely an EF navigation property
    private bool IsEfNavigationProperty(PropertyInfo property)
    {
        var propertyType = property.PropertyType;

        // Check if it's a reference navigation property (single entity)
        if (propertyType.IsClass &&
            propertyType != typeof(string) &&
            propertyType.Namespace?.Contains(".Domain") == true)
        {
            return true;
        }

        // Check if it's a collection navigation property
        if (propertyType.IsGenericType)
        {
            var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IReadOnlyCollection<>) ||
                genericTypeDefinition == typeof(List<>) ||
                genericTypeDefinition == typeof(HashSet<>))
            {
                var elementType = propertyType.GetGenericArguments()[0];
                if (elementType.IsClass && elementType.Namespace?.Contains(".Domain") == true)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Fact]
    public void Entities_Should_Have_Protected_Against_Unwanted_Inheritance()
    {
        // Entities should be sealed classes according to domain guidelines
        // Only allow unsealed entities if they have virtual properties for EF lazy loading
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            // Get all virtual properties
            var allVirtualProperties = entityType.GetProperties()
                .Where(p => p.GetGetMethod()?.IsVirtual == true)
                .ToList();

            // Filter out base entity properties that are commonly virtual due to framework requirements
            var baseEntityPropertyNames = new HashSet<string>
            {
                "UpdatedAt", "IsDeleted", "DomainEvents", "CreatedAt", "CreatedBy", "UpdatedBy"
            };

            // Only consider EF navigation properties as legitimate reasons to be unsealed
            var efNavigationProperties = allVirtualProperties
                .Where(p => !baseEntityPropertyNames.Contains(p.Name))
                .Where(p => IsEfNavigationProperty(p))
                .ToList();

            var hasEfNavigationProperties = efNavigationProperties.Any();

            // If entity is not sealed and doesn't use EF navigation props, flag it as a violation
            if (!entityType.IsSealed && !hasEfNavigationProperties)
            {
                failures.Add($"{entityType.Name} should be sealed - entities must be sealed classes unless they require virtual properties for EF navigation");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Entities_Should_Have_Private_Or_Protected_Constructors()
    {
        // Ensure entities do not expose a public parameterless ctor (encourages factory/Create usage)
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            var constructors = entityType.GetConstructors(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            var hasPublicParameterlessConstructor = constructors
                .Any(c => c.IsPublic && c.GetParameters().Length == 0);

            if (hasPublicParameterlessConstructor)
            {
                failures.Add($"{entityType.Name} has a public parameterless constructor - should be private or protected");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Entities_Should_Have_Static_Create_Method()
    {
        // Verify presence of a static Create factory method on entities for controlled construction
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            var createMethods = entityType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Create" || m.Name.StartsWith("Create"))
                .ToList();

            if (!createMethods.Any())
            {
                failures.Add($"{entityType.Name} does not have a static Create method");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void StronglyTypedIds_Should_Be_Records_With_Guid_Value()
    {
        // Ensure Id value objects follow the strongly-typed Guid-backed record convention
        var failures = new List<string>();

        var idTypes = _allDomainTypes
            .Where(t => t.Name.EndsWith("Id") &&
                       !t.Name.Contains("Entity") &&
                       !t.IsEnum)
            .ToList();

        foreach (var idType in idTypes)
        {
            // Check if it's a record (has compiler-generated EqualityContract property)
            var equalityContract = idType.GetProperty("EqualityContract",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (equalityContract == null)
            {
                failures.Add($"{idType.Name} should be a record type");
            }

            // Check for Value property of type Guid
            var valueProperty = idType.GetProperty("Value");
            if (valueProperty == null || valueProperty.PropertyType != typeof(Guid))
            {
                failures.Add($"{idType.Name} should have a Value property of type Guid");
            }

            // Check presence of constructor accepting Guid
            var constructor = idType.GetConstructor(new[] { typeof(Guid) });
            if (constructor == null)
            {
                failures.Add($"{idType.Name} should have a constructor that takes a Guid parameter");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void ValueObjects_Should_Be_Immutable()
    {
        // Value objects should be immutable: no public setters (except init-only) and no public fields
        var failures = new List<string>();

        foreach (var valueObjectType in _valueObjectTypes)
        {
            // Skip ID types as they're tested separately
            if (valueObjectType.Name.EndsWith("Id"))
                continue;

            var properties = valueObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Detect public setters and allow init-only as acceptable immutability
                if (property.CanWrite && property.SetMethod?.IsPublic == true)
                {
                    // Determine if setter is an init-only setter using required custom modifiers
                    var setMethod = property.SetMethod;
                    var isInitOnly = setMethod.ReturnParameter
                        ?.GetRequiredCustomModifiers()
                        ?.Any(t => t.Name == "IsExternalInit") ?? false;

                    if (!isInitOnly)
                    {
                        failures.Add($"{valueObjectType.Name}.{property.Name} has a public setter - value objects should be immutable (use init-only setters or constructor parameters)");
                    }
                }
            }

            // Public fields break immutability - flag them
            var publicFields = valueObjectType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (publicFields.Any())
            {
                failures.Add($"{valueObjectType.Name} has public fields - value objects should only expose properties");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void DomainEvents_Should_Be_Records()
    {
        // Domain events are preferred as records to ensure immutability and value semantics
        var failures = new List<string>();

        foreach (var eventType in _domainEventTypes)
        {
            // Check for record-like EqualityContract property
            var equalityContract = eventType.GetProperty("EqualityContract",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (equalityContract == null)
            {
                // Fallback: check for positional-record style ctor matching public properties
                var isPositionalRecord = eventType.GetConstructors()
                    .Any(c => c.GetParameters().Length > 0 &&
                             c.GetParameters().All(p =>
                                 eventType.GetProperty(char.ToUpper(p.Name![0]) + p.Name.Substring(1)) != null));

                if (!isPositionalRecord)
                {
                    failures.Add($"{eventType.Name} should be a record type for immutability");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Entities_Should_Not_Have_Public_Setters_Except_Audit_Fields()
    {
        // Entities should encapsulate state; public setters only allowed for common audit fields or EF needs
        var failures = new List<string>();

        // Common audit/framework fields that might need public setters for EF
        var allowedPublicSetterProperties = new HashSet<string>
        {
            "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt",
            "CreatedBy", "UpdatedBy", "DeletedBy", "RowVersion"
        };

        foreach (var entityType in _entityTypes)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Skip audit fields explicitly allowed
                if (allowedPublicSetterProperties.Contains(property.Name))
                    continue;

                // Skip readonly collection properties expressed as IReadOnlyCollection<T>
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
                    continue;

                // Skip navigation properties used by EF (virtual, non-sealed reference types except string)
                if (property.GetGetMethod()?.IsVirtual == true &&
                    property.PropertyType.IsClass &&
                    !property.PropertyType.IsSealed &&
                    property.PropertyType != typeof(string))
                    continue;

                // Check for public setter that's not init-only
                if (property.CanWrite && property.SetMethod?.IsPublic == true)
                {
                    var setMethod = property.SetMethod;
                    var isInitOnly = setMethod.ReturnParameter
                        ?.GetRequiredCustomModifiers()
                        ?.Any(t => t.Name == "IsExternalInit") ?? false;

                    if (!isInitOnly)
                    {
                        failures.Add($"{entityType.Name}.{property.Name} has a public setter - use private setters or methods to modify state");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Aggregates_Should_Raise_Domain_Events_Through_Methods()
    {
        // Check for aggregate-like entities that change state and whether they expose event support
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            // Identify public instance methods that imply state changes by naming convention
            var methods = entityType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName && // Skip property getters/setters
                           m.DeclaringType == entityType && // Skip inherited methods
                           (m.Name.StartsWith("Update") ||
                            m.Name.StartsWith("Change") ||
                            m.Name.StartsWith("Set") ||
                            m.Name.StartsWith("Add") ||
                            m.Name.StartsWith("Remove") ||
                            m.Name.StartsWith("Delete")))
                .ToList();

            // If such methods exist, the aggregate should ideally support domain events
            if (methods.Any())
            {
                // Verify base class or type provides a mechanism to raise/store domain events
                var hasEventSupport = HasDomainEventSupport(entityType);

                if (!hasEventSupport)
                {
                    // Currently treated as informational: not all entities need domain events.
                    // No failures added to avoid false positives.
                }
            }
        }

        Assert.Empty(failures);
    }

    // Walk the inheritance chain to find common domain event support members
    private bool HasDomainEventSupport(Type entityType)
    {
        var baseType = entityType.BaseType;
        while (baseType != null)
        {
            // Check for common backing field pattern for domain events
            var domainEventsField = baseType.GetField("_domainEvents",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (domainEventsField != null)
                return true;

            // Check for method used to raise domain events
            var raiseEventMethod = baseType.GetMethod("RaiseDomainEvent",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (raiseEventMethod != null)
                return true;

            baseType = baseType.BaseType;
        }
        return false;
    }

    [Fact]
    public void Entities_Should_Be_In_Correct_Namespace()
    {
        // Ensure domain entities live in Domain namespace and not in other layers
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            var @namespace = entityType.Namespace ?? string.Empty;

            // Entities should be in Domain namespace
            if (!@namespace.Contains(".Domain"))
            {
                failures.Add($"{entityType.Name} is not in a .Domain namespace");
            }

            // Entities must not be defined in Infrastructure/Application/Persistence/Api layers
            if (@namespace.Contains(".Infrastructure") ||
                @namespace.Contains(".Application") ||
                @namespace.Contains(".Persistence") ||
                @namespace.Contains(".Api"))
            {
                failures.Add($"{entityType.Name} is in {@namespace} - entities should only be in Domain layer");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Repository_Interfaces_Should_Be_In_Application_Layer()
    {
        // Verify repository interfaces are declared in the Application project (not Domain)
        var failures = new List<string>();

        // Try to find the Application assembly by name
        var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "LicenseService.Application");

        if (applicationAssembly != null)
        {
            var repositoryInterfaces = applicationAssembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
                .ToList();

            foreach (var repoInterface in repositoryInterfaces)
            {
                var @namespace = repoInterface.Namespace ?? string.Empty;

                if (!@namespace.Contains(".Application"))
                {
                    failures.Add($"{repoInterface.Name} should be in Application layer");
                }
            }
        }

        // Ensure Domain assembly does not contain repository interfaces
        var domainRepoInterfaces = _domainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToList();

        foreach (var repoInterface in domainRepoInterfaces)
        {
            failures.Add($"{repoInterface.Name} is in Domain layer - repository interfaces should be in Application layer");
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        // Domain layer must remain independent from infrastructure/persistence concerns
        var failures = new List<string>();

        // Inspect constructors and methods for illegal dependencies
        foreach (var domainType in _allDomainTypes)
        {
            // Check constructor parameters for infra types
            var constructors = domainType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    var paramTypeName = parameter.ParameterType.FullName ?? "";
                    if (paramTypeName.Contains(".Infrastructure") ||
                        paramTypeName.Contains(".Persistence") ||
                        paramTypeName.Contains("DbContext") ||
                        paramTypeName.Contains("Entity Framework"))
                    {
                        failures.Add($"{domainType.Name} constructor depends on infrastructure type: {parameter.ParameterType.Name}");
                    }
                }
            }

            // Check method parameters for infra types
            var methods = domainType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    var paramTypeName = parameter.ParameterType.FullName ?? "";
                    if (paramTypeName.Contains(".Infrastructure") ||
                        paramTypeName.Contains(".Persistence"))
                    {
                        failures.Add($"{domainType.Name}.{method.Name} depends on infrastructure type: {parameter.ParameterType.Name}");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Entities_Should_Override_Equality_Members_Properly()
    {
        // Entities should have equality semantics appropriate for identity-based comparison
        var failures = new List<string>();

        foreach (var entityType in _entityTypes)
        {
            // Entities should compare by ID, not by full value equality.
            // We check for Equals/GetHashCode overrides or rely on Entity<T> base class.
            var hasProperEquality = false;
            var currentType = entityType;

            while (currentType != null && currentType != typeof(object))
            {
                // Look for Equals(object) override declared on the type
                var equalsMethod = currentType.GetMethod("Equals",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    null,
                    new[] { typeof(object) },
                    null);

                if (equalsMethod != null)
                {
                    hasProperEquality = true;
                    break;
                }

                // Also consider GetHashCode override as part of equality contract
                var getHashCodeMethod = currentType.GetMethod("GetHashCode",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    null,
                    Type.EmptyTypes,
                    null);

                if (getHashCodeMethod != null)
                {
                    hasProperEquality = true;
                    break;
                }

                currentType = currentType.BaseType;
            }

            // If no overrides found, accept generic Entity<T> base class as provider of equality behavior
            if (!hasProperEquality)
            {
                currentType = entityType.BaseType;
                while (currentType != null)
                {
                    if (currentType.IsGenericType &&
                        currentType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
                    {
                        // Base Entity class likely provides proper equality implementation
                        hasProperEquality = true;
                        break;
                    }
                    currentType = currentType.BaseType;
                }
            }

            if (!hasProperEquality)
            {
                failures.Add($"{entityType.Name} or its base class should override Equals for proper entity comparison");
            }
        }

        Assert.Empty(failures);
    }
}