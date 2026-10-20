namespace BusinessPluginAssistant.Models;

public class CustomerBalanceResult
{
    public bool Found { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Balance { get; set; }
    public string Message { get; set; } = "";
}

