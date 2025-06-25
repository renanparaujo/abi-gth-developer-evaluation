using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime CancelledAt { get; }
    public string CancellationReason { get; }

    public SaleCancelledEvent(Sale sale)
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        CancelledAt = sale.CancelledAt ?? DateTime.UtcNow;
        CancellationReason = sale.CancellationReason ?? string.Empty;
    }
}