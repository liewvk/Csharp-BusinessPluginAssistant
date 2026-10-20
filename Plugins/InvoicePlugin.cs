using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace BusinessPluginAssistant.Plugins;

public class InvoicePlugin
{
    [KernelFunction("calculate_invoice")]
    [Description(
        "Calculates invoice totals using quantity, " +
        "unit price, discount percentage, and tax percentage.")]
    public string CalculateInvoice(
        [Description("Number of items")]
        int quantity,

        [Description("Price per item in RM")]
        decimal unitPrice,

        [Description("Discount percentage, e.g. 10 for 10%")]
        decimal discountPercent,

        [Description("Tax percentage, e.g. 6 for 6%")]
        decimal taxPercent)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.");

        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException(
                "Invalid discount percentage.");

        if (taxPercent < 0 || taxPercent > 100)
            throw new ArgumentException(
                "Invalid tax percentage.");

        decimal subtotal = quantity * unitPrice;

        decimal discount = Math.Round(
            subtotal * discountPercent / 100m, 2);

        decimal taxableAmount = subtotal - discount;

        decimal tax = Math.Round(
            taxableAmount * taxPercent / 100m, 2);

        decimal total = taxableAmount + tax;

        return $"""
            Invoice Summary
            Subtotal: RM{subtotal:F2}
            Discount: RM{discount:F2}
            Tax: RM{tax:F2}
            Final Total: RM{total:F2}
            """;
    }
}

