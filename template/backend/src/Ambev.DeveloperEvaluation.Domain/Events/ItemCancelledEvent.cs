using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public DateTime CancelledAt { get; }
    public string CancellationReason { get; }

    public ItemCancelledEvent(SaleItem item)
    {
        SaleId = item.SaleId;
        ItemId = item.Id;
        ProductId = item.ProductId;
        ProductName = item.ProductName;
        CancelledAt = item.CancelledAt ?? DateTime.UtcNow;
        CancellationReason = item.CancellationReason ?? string.Empty;
    }
}