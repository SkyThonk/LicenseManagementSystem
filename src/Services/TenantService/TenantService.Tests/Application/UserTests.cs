using TenantService.Domain.User;
using TenantService.Domain.Tenant;

namespace TenantService.Tests.Application;

public class UserTests
{
    [Fact]
    public void Create_Should_Return_Active_User_With_Correct_Properties()
    {
        // Arrange
        var email = "user@example.com";
        var passwordHash = "hashedPassword123";
        var firstName = "John";
        var lastName = "Doe";
        var role = Role.User;
        var tenantId = new TenantId(Guid.NewGuid());

        // Act
        var user = User.Create(email, passwordHash, firstName, lastName, role, tenantId);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(role, user.Role);
        Assert.Equal(tenantId, user.TenantId);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Create_Should_Generate_Unique_UserId()
    {
        // Arrange
        var tenantId = new TenantId(Guid.NewGuid());

        // Act
        var user1 = User.Create("user1@example.com", "hash1", "John", "Doe", Role.User, tenantId);
        var user2 = User.Create("user2@example.com", "hash2", "Jane", "Doe", Role.User, tenantId);

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void UpdateEmail_Should_Change_Email()
    {
        // Arrange
        var user = CreateTestUser();
        var newEmail = "newemail@example.com";

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void UpdateName_Should_Change_FirstName_And_LastName()
    {
        // Arrange
        var user = CreateTestUser();
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        user.UpdateName(newFirstName, newLastName);

        // Assert
        Assert.Equal(newFirstName, user.FirstName);
        Assert.Equal(newLastName, user.LastName);
    }

    [Fact]
    public void UpdateRole_Should_Change_Role()
    {
        // Arrange
        var user = CreateTestUser();
        Assert.Equal(Role.User, user.Role);

        // Act
        user.UpdateRole(Role.Admin);

        // Assert
        Assert.Equal(Role.Admin, user.Role);
    }

    [Fact]
    public void UpdatePassword_Should_Change_PasswordHash()
    {
        // Arrange
        var user = CreateTestUser();
        var newPasswordHash = "newHashedPassword456";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        Assert.Equal(newPasswordHash, user.PasswordHash);
    }

    [Fact]
    public void Block_Should_Set_IsActive_To_False()
    {
        // Arrange
        var user = CreateTestUser();
        Assert.True(user.IsActive);

        // Act
        user.Block();

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Unblock_Should_Set_IsActive_To_True()
    {
        // Arrange
        var user = CreateTestUser();
        user.Block();
        Assert.False(user.IsActive);

        // Act
        user.Unblock();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void UpdateEmail_Should_Update_UpdatedAt_Timestamp()
    {
        // Arrange
        var user = CreateTestUser();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.UpdateEmail("newemail@example.com");

        // Assert
        Assert.True(user.UpdatedAt >= originalUpdatedAt);
    }

    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Admin)]
    [InlineData(Role.TenantAdmin)]
    public void Create_Should_Allow_All_Role_Types(Role role)
    {
        // Arrange
        var tenantId = new TenantId(Guid.NewGuid());

        // Act
        var user = User.Create("user@example.com", "hash", "John", "Doe", role, tenantId);

        // Assert
        Assert.Equal(role, user.Role);
    }

    private static User CreateTestUser()
    {
        var tenantId = new TenantId(Guid.NewGuid());
        return User.Create("test@example.com", "hashedPassword", "Test", "User", Role.User, tenantId);
    }
}
