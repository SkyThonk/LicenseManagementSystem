using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Tests.Application;

public class LicenseTypeTests
{
    [Fact]
    public void Create_Should_Return_LicenseType_With_Correct_Properties()
    {
        // Arrange
        var name = "Medical License";
        var description = "License for medical professionals";
        var feeAmount = 150.00m;

        // Act
        var licenseType = LicenseType.Create(name, description, feeAmount);

        // Assert
        Assert.NotNull(licenseType);
        Assert.Equal(name, licenseType.Name);
        Assert.Equal(description, licenseType.Description);
        Assert.Equal(feeAmount, licenseType.FeeAmount);
    }

    [Fact]
    public void Create_Should_Throw_When_Name_Is_Empty()
    {
        // Arrange
        var name = "";
        var description = "Test description";
        var feeAmount = 100.00m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => LicenseType.Create(name, description, feeAmount));
    }

    [Fact]
    public void Create_Should_Throw_When_FeeAmount_Is_Negative()
    {
        // Arrange
        var name = "Test License";
        var description = "Test description";
        var feeAmount = -50.00m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => LicenseType.Create(name, description, feeAmount));
    }

    [Fact]
    public void Update_Should_Change_Properties()
    {
        // Arrange
        var licenseType = LicenseType.Create(
            "Original Name",
            "Original Description",
            100.00m
        );

        // Act
        licenseType.Update("Updated Name", "Updated Description", 200.00m);

        // Assert
        Assert.Equal("Updated Name", licenseType.Name);
        Assert.Equal("Updated Description", licenseType.Description);
        Assert.Equal(200.00m, licenseType.FeeAmount);
    }
}
