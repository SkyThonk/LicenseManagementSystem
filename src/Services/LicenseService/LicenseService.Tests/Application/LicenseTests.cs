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
        var tenantId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        var licenseTypeId = new LicenseTypeId(Guid.NewGuid());

        // Act
        var license = License.Create(tenantId, applicantId, licenseTypeId);

        // Assert
        Assert.NotNull(license);
        Assert.Equal(tenantId, license.TenantId);
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
            Guid.NewGuid(),
            new LicenseTypeId(Guid.NewGuid())
        );
        license.Submit();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => license.Submit());
    }
}
