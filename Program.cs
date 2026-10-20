using System.Text.Json;
using BusinessPluginAssistant.Plugins;
using BusinessPluginAssistant.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

Console.Title = "C# Business Plugin Assistant";


var builder = Kernel.CreateBuilder();

// Increase the timeout from 100 seconds to 10 minutes.
using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(10)
};

// Connect Semantic Kernel to local Ollama.
builder.AddOpenAIChatCompletion(
    modelId: "qwen3.5:9b",
    endpoint: new Uri("http://localhost:11434/v1"),
    apiKey: "ollama",
    httpClient: httpClient);

Kernel kernel = builder.Build();

kernel.Plugins.AddFromType<InvoicePlugin>("Invoice");
kernel.Plugins.AddFromType<ProductPlugin>("Product");
kernel.Plugins.AddFromType<CustomerPlugin>("Customer");

IChatCompletionService chatService =
    kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("==================================");
Console.WriteLine("     BUSINESS PLUGIN ASSISTANT");
Console.WriteLine("       Visual Studio 2026");
Console.WriteLine("       Semantic Kernel + Ollama");
Console.WriteLine("==================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Calculate Invoice");
    Console.WriteLine("2. Get Product Information");
    Console.WriteLine("3. List Low-Stock Products");
    Console.WriteLine("4. Get Customer Balance");
    Console.WriteLine("5. Ask Qwen 3.5");
    Console.WriteLine("0. Exit");
    Console.WriteLine();

    Console.Write("Select option: ");
    string? choice = Console.ReadLine();

    if (choice == "0")
        break;

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("Quantity: ");
                int quantity = int.Parse(
                    Console.ReadLine() ?? "0");

                Console.Write("Unit price: ");
                decimal price = decimal.Parse(
                    Console.ReadLine() ?? "0");

                Console.Write("Discount (%): ");
                decimal discount = decimal.Parse(
                    Console.ReadLine() ?? "0");

                Console.Write("Tax (%): ");
                decimal tax = decimal.Parse(
                    Console.ReadLine() ?? "0");

                var invoice = await kernel.InvokeAsync(
                    "Invoice",
                    "calculate_invoice",
                    new KernelArguments
                    {
                        ["quantity"] = quantity,
                        ["unitPrice"] = price,
                        ["discountPercent"] = discount,
                        ["taxPercent"] = tax
                    });

                Console.WriteLine(invoice);
                break;

            case "2":
                Console.Write("Product name: ");
                string productName =
                    Console.ReadLine() ?? "";

                var product = await kernel.InvokeAsync(
                    "Product",
                    "get_product",
                    new KernelArguments
                    {
                        ["productName"] = productName
                    });

                var productInfo =
                    product.GetValue<ProductInfo>();

                if (productInfo == null)
                {
                    Console.WriteLine("Product not found.");
                }
                else
                {
                    Console.WriteLine(
                        JsonSerializer.Serialize(
                            productInfo,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));
                }
                break;

            case "3":
                Console.Write("Stock threshold: ");

                int threshold = int.Parse(
                    Console.ReadLine() ?? "0");

                var stock = await kernel.InvokeAsync(
                    "Product",
                    "list_low_stock",
                    new KernelArguments
                    {
                        ["threshold"] = threshold
                    });

                Console.WriteLine(stock);
                break;

            case "4":
                Console.Write("Customer name: ");

                string customerName =
                    Console.ReadLine() ?? "";

                var customer = await kernel.InvokeAsync(
                    "Customer",
                    "get_customer_balance",
                    new KernelArguments
                    {
                        ["customerName"] = customerName
                    });

                var balance =
                    customer.GetValue<CustomerBalanceResult>();

                if (balance == null || !balance.Found)
                {
                    Console.WriteLine("Customer not found.");
                }
                else
                {
                    Console.WriteLine(
                        $"Customer: {balance.CustomerName}");

                    Console.WriteLine(
                        $"Outstanding Balance: RM{balance.Balance:F2}");
                }
                break;

            case "5":
                Console.Write("Enter your question: ");

                string question =
                    Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(question))
                    break;

                var history = new ChatHistory();

                history.AddSystemMessage(
                    "You are a helpful C# programming " +
                    "and business application assistant.");

                history.AddUserMessage(question);

                Console.WriteLine("Qwen is thinking...");

                var answer =
                    await chatService.GetChatMessageContentAsync(
                        history,
                        kernel: kernel);

                Console.WriteLine();
                Console.WriteLine("Assistant:");
                Console.WriteLine(answer.Content);
                break;

            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid numeric input.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

Console.WriteLine("Goodbye!");

