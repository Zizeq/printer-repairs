using PrinterRepair.Identification.Domain.Aggregates;

namespace PrinterRepair.Identification.Domain.Tests;

public class RegisteredPrinterTests
{
    [Fact]
    public void AssignToClient_WhenDetailsAreValid_ShouldCreateRegisteredPrinter()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        string serial = "SN-98765";
        string model = "ProPrint 2000";

        // Act
        var printer = RegisteredPrinter.AssignToClient(clientId, serial, model);

        // Assert
        Assert.NotEqual(Guid.Empty, printer.Id);
        Assert.Equal(clientId, printer.ClientAccountId);
        Assert.Equal(serial, printer.SerialNumber);
        Assert.Equal(model, printer.PrinterModel);
    }

    [Theory]
    [InlineData("", "ValidModel")]
    [InlineData("ValidSerial", "")]
    [InlineData(null, "ValidModel")]
    [InlineData("ValidSerial", null)]
    public void AssignToClient_WhenDetailsAreInvalid_ShouldThrowArgumentException(string serialNumber, string printerModel)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            RegisteredPrinter.AssignToClient(Guid.NewGuid(), serialNumber, printerModel));

        Assert.Equal("Printer details cannot be empty.", exception.Message);
    }
}