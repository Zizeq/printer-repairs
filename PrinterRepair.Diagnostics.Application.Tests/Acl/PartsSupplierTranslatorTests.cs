using System.Globalization;
using PrinterRepair.Diagnostics.Application.Ports;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using PrinterRepair.Diagnostics.Domain.ValueObjects;
using PrinterRepair.Diagnostics.Infrastructure.Acl;
using PrinterRepair.Diagnostics.Infrastructure.Acl.ExternalContracts;
using Xunit;

namespace PrinterRepair.Diagnostics.Application.Tests.Acl;

public sealed class PartsSupplierTranslatorTests
{
    private readonly PartsSupplierTranslator _sut = new();

    [Fact]
    public void ToSupplierDto_WhenValidPlnOrderUnderPolishCulture_ShouldFormatWithDotAndUppercaseCode()
    {
        // Arrange
        var previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");

        try
        {
            var quantity = PartQuantity.Of(3);
            var maxPrice = Money.Of(149.50m, "PLN");

            // Act
            var dto = _sut.ToSupplierDto("  fuser-hp-400  ", quantity, maxPrice);

            // Assert
            Assert.Equal("FUSER-HP-400", dto.PART_CD);
            Assert.Equal(3, dto.QTY_REQ);
            Assert.Equal("149.50", dto.MAX_PRC_S);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void ToSupplierDto_WhenCurrencyIsNotPln_ShouldThrowPartsSupplierIntegrationException()
    {
        // Arrange
        var quantity = PartQuantity.Of(1);
        var eurPrice = Money.Of(50.00m, "EUR");

        // Act & Assert
        Assert.Throws<PartsSupplierIntegrationException>(() =>
            _sut.ToSupplierDto("FUSER-HP-400", quantity, eurPrice));
    }

    [Fact]
    public void ToSupplierOrderRef_WhenResponseSucceeded_ShouldReturnMappedReference()
    {
        // Arrange
        var response = new PART_RES_DTO { ERR_CD = 0, EXT_ORD_NO = "SUP/2026/00412" };

        // Act
        var orderRef = _sut.ToSupplierOrderRef(response);

        // Assert
        Assert.Equal("SUP/2026/00412", orderRef.ExternalOrderId);
    }

    [Fact]
    public void ToException_WhenErrorCodeIsMinus10_ShouldReturnInsufficientStockException()
    {
        // Act
        var exception = _sut.ToException(-10, "FUSER-HP-400");

        // Assert
        var domainEx = Assert.IsType<InsufficientStockException>(exception);
        Assert.Equal("diagnostics.insufficient_stock", domainEx.Code);
    }

    [Fact]
    public void ToException_WhenErrorCodeIsMinus20_ShouldReturnObsoletePartException()
    {
        // Act
        var exception = _sut.ToException(-20, "FUSER-HP-400");

        // Assert
        var domainEx = Assert.IsType<ObsoletePartException>(exception);
        Assert.Equal("diagnostics.obsolete_part", domainEx.Code);
    }

    [Fact]
    public void ToException_WhenErrorCodeIsUnknown_ShouldReturnPartsSupplierIntegrationExceptionWithRawCode()
    {
        // Act
        var exception = _sut.ToException(-99, "FUSER-HP-400");

        // Assert
        var integrationEx = Assert.IsType<PartsSupplierIntegrationException>(exception);
        Assert.Contains("-99", integrationEx.Message);
    }
}