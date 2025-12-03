using TenantService.Domain.Common.ValueObjects;
using TenantService.Domain.Common.Exceptions;

namespace TenantService.Tests.Application;

public class ValueObjectTests
{
    #region Address Tests

    [Fact]
    public void Address_Create_Should_Return_Address_With_Correct_Properties()
    {
        // Arrange
        var addressLine1 = "123 Main St";
        var addressLine2 = "Suite 100";
        var city = "Austin";
        var state = "TX";
        var postalCode = "78701";
        var countryCode = "US";

        // Act
        var address = Address.Create(addressLine1, addressLine2, city, state, postalCode, countryCode);

        // Assert
        Assert.Equal(addressLine1, address.AddressLineOne);
        Assert.Equal(addressLine2, address.AddressLineTwo);
        Assert.Equal(city, address.City);
        Assert.Equal(state, address.State);
        Assert.Equal(postalCode, address.PostalCode);
        Assert.Equal(countryCode, address.CountryCode);
    }

    [Fact]
    public void Address_Create_Should_Throw_When_AddressLineOne_Is_Empty()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Address.Create("", null, "Austin", "TX"));
    }

    [Fact]
    public void Address_Create_Should_Throw_When_City_Is_Empty()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Address.Create("123 Main St", null, "", "TX"));
    }

    [Fact]
    public void Address_Create_Should_Throw_When_State_Is_Empty()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Address.Create("123 Main St", null, "Austin", ""));
    }

    [Fact]
    public void Address_Create_Should_Allow_Null_AddressLineTwo()
    {
        // Act
        var address = Address.Create("123 Main St", null, "Austin", "TX");

        // Assert
        Assert.Null(address.AddressLineTwo);
    }

    #endregion

    #region PhoneNumber Tests

    [Fact]
    public void PhoneNumber_Create_Should_Return_PhoneNumber_With_Correct_Properties()
    {
        // Arrange
        var countryCode = "+1";
        var number = "5551234567";

        // Act
        var phoneNumber = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.Equal(countryCode, phoneNumber.CountryCode);
        Assert.Equal(number, phoneNumber.Number);
    }

    [Fact]
    public void PhoneNumber_FullNumber_Should_Return_Combined_String()
    {
        // Arrange
        var countryCode = "+1";
        var number = "5551234567";

        // Act
        var phoneNumber = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.Equal("+15551234567", phoneNumber.FullNumber);
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_CountryCode_Is_Empty()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            PhoneNumber.Create("", "5551234567"));
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_Number_Is_Empty()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            PhoneNumber.Create("+1", ""));
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_CountryCode_Does_Not_Start_With_Plus()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            PhoneNumber.Create("1", "5551234567"));
    }

    [Theory]
    [InlineData("+1", "5551234567")]
    [InlineData("+44", "7911123456")]
    [InlineData("+91", "9876543210")]
    public void PhoneNumber_Create_Should_Accept_Valid_Country_Codes(string countryCode, string number)
    {
        // Act
        var phoneNumber = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.Equal(countryCode, phoneNumber.CountryCode);
        Assert.Equal(number, phoneNumber.Number);
    }

    #endregion
}
