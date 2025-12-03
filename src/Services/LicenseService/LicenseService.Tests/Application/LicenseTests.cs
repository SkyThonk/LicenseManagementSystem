using LicenseService.Domain.Licenses;
using LicenseService.Domain.LicenseTypes;
using LicenseService.Domain.Common.Enums;

namespace LicenseService.Tests.Application;

public class LicenseTests
{
    [Fact]
    public void Create_Should_Return_License_With_Draft_Status()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var licenseTypeId = new LicenseTypeId(Guid.NewGuid());

        // Act
        var license = License.Create(applicantId, licenseTypeId);

        // Assert
        Assert.NotNull(license);
        Assert.Equal(applicantId, license.ApplicantId);
        Assert.Equal(licenseTypeId, license.LicenseTypeId);
        Assert.Equal(LicenseStatus.Draft, license.Status);
    }

    [Fact]
    public void Submit_Should_Change_Status_To_Submitted()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );

        // Act
        license.Submit();

        // Assert
        Assert.Equal(LicenseStatus.Submitted, license.Status);
    }

    [Fact]
    public void Submit_Should_Throw_When_Not_Draft()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );
        license.Submit();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => license.Submit());
    }

    [Fact]
    public void Approve_Should_Change_Status_To_Approved_When_Submitted()
    {
        // Arrange
        var license = CreateSubmittedLicense();

        // Act
        license.Approve();

        // Assert
        Assert.Equal(LicenseStatus.Approved, license.Status);
        Assert.NotNull(license.ApprovedAt);
    }

    [Fact]
    public void Approve_Should_Set_ExpiryDate_When_Provided()
    {
        // Arrange
        var license = CreateSubmittedLicense();
        var expiryDate = DateTime.UtcNow.AddYears(1);

        // Act
        license.Approve(expiryDate);

        // Assert
        Assert.Equal(expiryDate, license.ExpiryDate);
    }

    [Fact]
    public void Approve_Should_Throw_When_Status_Is_Draft()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => license.Approve());
    }

    [Fact]
    public void Reject_Should_Change_Status_To_Rejected()
    {
        // Arrange
        var license = CreateSubmittedLicense();

        // Act
        license.Reject();

        // Assert
        Assert.Equal(LicenseStatus.Rejected, license.Status);
    }

    [Fact]
    public void Reject_Should_Throw_When_Status_Is_Draft()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => license.Reject());
    }

    [Fact]
    public void Renew_Should_Update_ExpiryDate_When_Approved()
    {
        // Arrange
        var license = CreateApprovedLicense();
        var newExpiryDate = DateTime.UtcNow.AddYears(2);

        // Act
        license.Renew(newExpiryDate);

        // Assert
        Assert.Equal(LicenseStatus.Approved, license.Status);
        Assert.Equal(newExpiryDate, license.ExpiryDate);
    }

    [Fact]
    public void Renew_Should_Throw_When_Status_Is_Draft()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );
        var newExpiryDate = DateTime.UtcNow.AddYears(1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => license.Renew(newExpiryDate));
    }

    [Fact]
    public void UpdateStatus_Should_Change_Status()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );

        // Act
        license.UpdateStatus(LicenseStatus.UnderReview);

        // Assert
        Assert.Equal(LicenseStatus.UnderReview, license.Status);
    }

    [Fact]
    public void UpdateStatus_Should_Set_ApprovedAt_When_Approved()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );

        // Act
        license.UpdateStatus(LicenseStatus.Approved);

        // Assert
        Assert.NotNull(license.ApprovedAt);
    }

    [Fact]
    public void UpdateStatus_Should_Set_ExpiryDate_When_Provided_And_Approved()
    {
        // Arrange
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );
        var expiryDate = DateTime.UtcNow.AddYears(1);

        // Act
        license.UpdateStatus(LicenseStatus.Approved, expiryDate);

        // Assert
        Assert.Equal(expiryDate, license.ExpiryDate);
    }

    private static License CreateSubmittedLicense()
    {
        var license = License.Create(
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );
        license.Submit();
        return license;
    }

    private static License CreateApprovedLicense()
    {
        var license = CreateSubmittedLicense();
        license.Approve(DateTime.UtcNow.AddYears(1));
        return license;
    }
}
