namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}