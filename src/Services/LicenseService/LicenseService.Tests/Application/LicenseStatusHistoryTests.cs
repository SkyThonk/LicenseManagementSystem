using LicenseService.Domain.LicenseStatusHistory;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;

namespace LicenseService.Tests.Application;

public class LicenseStatusHistoryTests
{
    [Fact]
    public void Create_Should_Return_History_Entry_With_Correct_Properties()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var licenseId = new LicenseId(Guid.NewGuid());
        var oldStatus = LicenseStatus.Draft;
        var newStatus = LicenseStatus.Submitted;
        var changedBy = Guid.NewGuid();
        var remarks = "Submitted for review";

        // Act
        var historyEntry = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, oldStatus, newStatus, changedBy, remarks);

        // Assert
        Assert.NotNull(historyEntry);
        Assert.Equal(tenantId, historyEntry.TenantId);
        Assert.Equal(licenseId, historyEntry.LicenseId);
        Assert.Equal(oldStatus, historyEntry.OldStatus);
        Assert.Equal(newStatus, historyEntry.NewStatus);
        Assert.Equal(changedBy, historyEntry.ChangedBy);
        Assert.Equal(remarks, historyEntry.Remarks);
    }

    [Fact]
    public void Create_Should_Set_ChangedAt_To_Current_Time()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var licenseId = new LicenseId(Guid.NewGuid());
        var changedBy = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var historyEntry = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, LicenseStatus.Draft, LicenseStatus.Submitted, changedBy);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(historyEntry.ChangedAt, beforeCreation, afterCreation);
    }

    [Fact]
    public void Create_Should_Allow_Null_Remarks()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var licenseId = new LicenseId(Guid.NewGuid());
        var changedBy = Guid.NewGuid();

        // Act
        var historyEntry = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, LicenseStatus.Draft, LicenseStatus.Submitted, changedBy, null);

        // Assert
        Assert.Null(historyEntry.Remarks);
    }

    [Fact]
    public void Create_Should_Generate_Unique_Id()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var licenseId = new LicenseId(Guid.NewGuid());
        var changedBy = Guid.NewGuid();

        // Act
        var entry1 = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, LicenseStatus.Draft, LicenseStatus.Submitted, changedBy);
        var entry2 = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, LicenseStatus.Submitted, LicenseStatus.UnderReview, changedBy);

        // Assert
        Assert.NotEqual(entry1.Id, entry2.Id);
    }

    [Theory]
    [InlineData(LicenseStatus.Draft, LicenseStatus.Submitted)]
    [InlineData(LicenseStatus.Submitted, LicenseStatus.UnderReview)]
    [InlineData(LicenseStatus.UnderReview, LicenseStatus.Approved)]
    [InlineData(LicenseStatus.UnderReview, LicenseStatus.Rejected)]
    [InlineData(LicenseStatus.Approved, LicenseStatus.Expired)]
    public void Create_Should_Track_All_Valid_Status_Transitions(LicenseStatus oldStatus, LicenseStatus newStatus)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var licenseId = new LicenseId(Guid.NewGuid());
        var changedBy = Guid.NewGuid();

        // Act
        var historyEntry = LicenseStatusHistoryEntry.Create(
            tenantId, licenseId, oldStatus, newStatus, changedBy);

        // Assert
        Assert.Equal(oldStatus, historyEntry.OldStatus);
        Assert.Equal(newStatus, historyEntry.NewStatus);
    }
}
