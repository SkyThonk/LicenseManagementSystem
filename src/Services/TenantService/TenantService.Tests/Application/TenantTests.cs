using TenantService.Domain.Tenant;
using TenantService.Domain.Common.ValueObjects;

namespace TenantService.Tests.Application;

public class TenantTests
{
    [Fact]
    public void Create_Should_Return_Active_Tenant_With_Correct_Properties()
    {
        // Arrange
        var name = "Texas Medical Board";
        var agencyCode = "TMB001";
        var address = Address.Create("123 Main St", null, "Austin", "TX", "78701", "US");
        var phone = PhoneNumber.Create("+1", "5551234567");
        var email = "contact@tmb.gov";
        var description = "Texas Medical Board Agency";

        // Act
        var tenant = Tenant.Create(name, agencyCode, address, phone, email, description);

        // Assert
        Assert.NotNull(tenant);
        Assert.Equal(name, tenant.Name);
        Assert.Equal(agencyCode, tenant.AgencyCode);
        Assert.Equal(email, tenant.Email);
        Assert.Equal(description, tenant.Description);
        Assert.True(tenant.IsActive);
        Assert.Equal(address, tenant.Address);
        Assert.Equal(phone, tenant.Phone);
    }

    [Fact]
    public void Create_Should_Raise_TenantCreatedEvent()
    {
        // Arrange
        var name = "Texas Medical Board";
        var agencyCode = "TMB001";
        var address = Address.Create("123 Main St", null, "Austin", "TX", "78701", "US");
        var phone = PhoneNumber.Create("+1", "5551234567");
        var email = "contact@tmb.gov";

        // Act
        var tenant = Tenant.Create(name, agencyCode, address, phone, email);

        // Assert
        var domainEvents = tenant.DomainEvents;
        Assert.Single(domainEvents);
        Assert.IsType<Common.Domain.Events.TenantCreatedEvent>(domainEvents.First());
    }

    [Fact]
    public void Activate_Should_Set_IsActive_To_True()
    {
        // Arrange
        var tenant = CreateTestTenant();
        tenant.Deactivate();
        Assert.False(tenant.IsActive);

        // Act
        tenant.Activate();

        // Assert
        Assert.True(tenant.IsActive);
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_To_False()
    {
        // Arrange
        var tenant = CreateTestTenant();
        Assert.True(tenant.IsActive);

        // Act
        tenant.Deactivate();

        // Assert
        Assert.False(tenant.IsActive);
    }

    [Fact]
    public void Update_Should_Change_Tenant_Properties()
    {
        // Arrange
        var tenant = CreateTestTenant();
        var newName = "Updated Medical Board";
        var newDescription = "Updated Description";
        var newEmail = "updated@tmb.gov";

        // Act
        tenant.Update(name: newName, description: newDescription, email: newEmail);

        // Assert
        Assert.Equal(newName, tenant.Name);
        Assert.Equal(newDescription, tenant.Description);
        Assert.Equal(newEmail, tenant.Email);
    }

    [Fact]
    public void Update_Should_Raise_TenantUpdatedEvent()
    {
        // Arrange
        var tenant = CreateTestTenant();
        tenant.ClearDomainEvent(); // Clear initial creation event

        // Act
        tenant.Update(name: "Updated Name");

        // Assert
        var domainEvents = tenant.DomainEvents;
        Assert.Single(domainEvents);
        Assert.IsType<Common.Domain.Events.TenantUpdatedEvent>(domainEvents.First());
    }

    [Fact]
    public void Delete_Should_Set_IsDeleted_To_True()
    {
        // Arrange
        var tenant = CreateTestTenant();

        // Act
        tenant.Delete();

        // Assert
        Assert.True(tenant.IsDeleted);
    }

    [Fact]
    public void Delete_Should_Raise_TenantDeletedEvent()
    {
        // Arrange
        var tenant = CreateTestTenant();
        tenant.ClearDomainEvent(); // Clear initial creation event

        // Act
        tenant.Delete();

        // Assert
        var domainEvents = tenant.DomainEvents;
        Assert.Single(domainEvents);
        Assert.IsType<Common.Domain.Events.TenantDeletedEvent>(domainEvents.First());
    }

    private static Tenant CreateTestTenant()
    {
        var address = Address.Create("123 Main St", null, "Austin", "TX", "78701", "US");
        var phone = PhoneNumber.Create("+1", "5551234567");
        return Tenant.Create("Test Agency", "TA001", address, phone, "test@agency.gov");
    }
}
