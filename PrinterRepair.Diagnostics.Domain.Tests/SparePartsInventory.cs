using PrinterRepair.DiagnosticsAndRepair.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Domain.Tests;

public class SparePartsInventoryTests
{
    [Fact]
    public void ReservePart_WhenStockIsSufficient_ShouldDecreaseStock()
    {
        // Arrange
        var inventory = new SparePartsInventory(Guid.NewGuid(), "Fuser Unit", 5);

        // Act
        inventory.ReservePart(2);

        // Assert
        Assert.Equal(3, inventory.StockLevel);
    }

    [Fact]
    public void ReservePart_WhenStockIsInsufficient_ShouldThrowException()
    {
        // Arrange
        var inventory = new SparePartsInventory(Guid.NewGuid(), "Fuser Unit", 1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            inventory.ReservePart(2));

        Assert.Contains("out of stock", exception.Message);
    }
}