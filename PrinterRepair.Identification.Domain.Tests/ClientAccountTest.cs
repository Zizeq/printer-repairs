using PrinterRepair.Identification.Domain.Aggregates;

namespace PrinterRepair.Identification.Domain.Tests;

public class ClientAccountTests
{
    [Fact]
    public void RegisterNew_WhenIdentifierIsValid_ShouldCreateAccount()
    {
        // Arrange
        string validIdentifier = "NIP-1234567890";
        bool isB2B = true;

        // Act
        var account = ClientAccount.RegisterNew(validIdentifier, isB2B);

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(validIdentifier, account.ContactIdentifier);
        Assert.True(account.IsPriorityB2B);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void RegisterNew_WhenIdentifierIsNullOrWhitespace_ShouldThrowArgumentException(string invalidIdentifier)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ClientAccount.RegisterNew(invalidIdentifier));

        Assert.Equal("Contact identifier cannot be empty.", exception.Message);
    }
}