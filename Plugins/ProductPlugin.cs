using System.ComponentModel;
using BusinessPluginAssistant.Models;
using Microsoft.SemanticKernel;

namespace BusinessPluginAssistant.Plugins;

public class ProductPlugin
{
    private readonly List<ProductInfo> _products = new()
    {
        new ProductInfo
        {
            Name = "Laptop",
            Price = 3500m,
            StockQuantity = 15,
            Category = "Computers"
        },
        new ProductInfo
        {
            Name = "Monitor",
            Price = 850m,
            StockQuantity = 30,
            Category = "Displays"
        },
        new ProductInfo
        {
            Name = "Keyboard",
            Price = 120m,
            StockQuantity = 8,
            Category = "Accessories"
        }
    };

    [KernelFunction("get_product")]
    [Description(
        "Retrieves product price, stock, and category.")]
    public ProductInfo? GetProduct(
        [Description("Product name to search for")]
        string productName)
    {
        return _products.FirstOrDefault(p =>
            p.Name.Equals(
                productName,
                StringComparison.OrdinalIgnoreCase));
    }

    [KernelFunction("list_low_stock")]
    [Description(
        "Lists products below the specified stock quantity.")]
    public string ListLowStock(
        [Description("Stock quantity threshold")]
        int threshold)
    {
        if (threshold < 0)
            throw new ArgumentException(
                "Threshold cannot be negative.");

        var items = _products
            .Where(p => p.StockQuantity < threshold)
            .Select(p => $"{p.Name}: {p.StockQuantity}");

        string result = string.Join(", ", items);

        return string.IsNullOrEmpty(result)
            ? "No low-stock products found."
            : result;
    }
}

