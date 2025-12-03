using LicenseService.Domain.Renewals;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;

namespace LicenseService.Tests.Application;

public class RenewalTests
{
    [Fact]
    public void Create_Should_Return_Renewal_With_Pending_Status()
    {
        // Arrange
        var licenseId = new LicenseId(Guid.NewGuid());
        var renewalDate = DateTime.UtcNow.AddYears(1);

        // Act
        var renewal = Renewal.Create(licenseId, renewalDate);

        // Assert
        Assert.NotNull(renewal);
        Assert.Equal(licenseId, renewal.LicenseId);
        Assert.Equal(renewalDate, renewal.RenewalDate);
        Assert.Equal(RenewalStatus.Pending, renewal.Status);
        Assert.Null(renewal.ProcessedAt);
    }

    [Fact]
    public void StartProcessing_Should_Change_Status_To_Processing()
    {
        // Arrange
        var renewal = CreateTestRenewal();

        // Act
        renewal.StartProcessing();

        // Assert
        Assert.Equal(RenewalStatus.Processing, renewal.Status);
    }

    [Fact]
    public void StartProcessing_Should_Throw_When_Not_Pending()
    {
        // Arrange
        var renewal = CreateTestRenewal();
        renewal.StartProcessing();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => renewal.StartProcessing());
    }

    [Fact]
    public void Approve_Should_Change_Status_To_Approved()
    {
        // Arrange
        var renewal = CreateTestRenewal();
        renewal.StartProcessing();

        // Act
        renewal.Approve();

        // Assert
        Assert.Equal(RenewalStatus.Approved, renewal.Status);
        Assert.NotNull(renewal.ProcessedAt);
    }

    [Fact]
    public void Approve_Should_Throw_When_Not_Processing()
    {
        // Arrange
        var renewal = CreateTestRenewal();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => renewal.Approve());
    }

    [Fact]
    public void Reject_Should_Change_Status_To_Rejected()
    {
        // Arrange
        var renewal = CreateTestRenewal();
        renewal.StartProcessing();

        // Act
        renewal.Reject();

        // Assert
        Assert.Equal(RenewalStatus.Rejected, renewal.Status);
        Assert.NotNull(renewal.ProcessedAt);
    }

    [Fact]
    public void Reject_Should_Throw_When_Not_Processing()
    {
        // Arrange
        var renewal = CreateTestRenewal();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => renewal.Reject());
    }

    [Fact]
    public void Complete_Should_Change_Status_To_Completed()
    {
        // Arrange
        var renewal = CreateTestRenewal();
        renewal.StartProcessing();
        renewal.Approve();

        // Act
        renewal.Complete();

        // Assert
        Assert.Equal(RenewalStatus.Completed, renewal.Status);
    }

    [Fact]
    public void Complete_Should_Throw_When_Not_Approved()
    {
        // Arrange
        var renewal = CreateTestRenewal();
        renewal.StartProcessing();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => renewal.Complete());
    }

    [Fact]
    public void Fail_Should_Change_Status_To_Failed()
    {
        // Arrange
        var renewal = CreateTestRenewal();

        // Act
        renewal.Fail();

        // Assert
        Assert.Equal(RenewalStatus.Failed, renewal.Status);
        Assert.NotNull(renewal.ProcessedAt);
    }

    [Fact]
    public void Fail_Should_Work_From_Any_Status()
    {
        // Arrange
        var renewal1 = CreateTestRenewal();
        var renewal2 = CreateTestRenewal();
        renewal2.StartProcessing();

        // Act
        renewal1.Fail();
        renewal2.Fail();

        // Assert
        Assert.Equal(RenewalStatus.Failed, renewal1.Status);
        Assert.Equal(RenewalStatus.Failed, renewal2.Status);
    }

    private static Renewal CreateTestRenewal()
    {
        var licenseId = new LicenseId(Guid.NewGuid());
        return Renewal.Create(licenseId, DateTime.UtcNow.AddYears(1));
    }
}
