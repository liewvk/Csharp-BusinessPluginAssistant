using System.ComponentModel;
using BusinessPluginAssistant.Models;
using Microsoft.SemanticKernel;

namespace BusinessPluginAssistant.Plugins;

public class CustomerPlugin
{
    private readonly Dictionary<string, decimal> _balances =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["ABC Trading"] = 8500m,
            ["Bright Star Sdn Bhd"] = 3200m,
            ["City Tech Supplies"] = 0m
        };

    [KernelFunction("get_customer_balance")]
    [Description(
        "Gets the outstanding balance for a customer.")]
    public CustomerBalanceResult GetCustomerBalance(
        [Description("Customer name")]
        string customerName)
    {
        if (_balances.TryGetValue(
            customerName, out decimal balance))
        {
            return new CustomerBalanceResult
            {
                Found = true,
                CustomerName = customerName,
                Balance = balance,
                Message = "Customer found."
            };
        }

        return new CustomerBalanceResult
        {
            Found = false,
            CustomerName = customerName,
            Message = "Customer not found."
        };
    }
}
