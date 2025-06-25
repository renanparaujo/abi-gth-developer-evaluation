using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleItemStatus Status { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public virtual Sale Sale { get; set; } = null!;

    public SaleItem()
    {
        Status = SaleItemStatus.Active;
    }

    public void Cancel(string reason)
    {
        Status = SaleItemStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
    }

    public void CalculateTotal()
    {
        var subtotal = Quantity * UnitPrice;
        TotalAmount = subtotal - Discount;
    }

    public void ApplyDiscount()
    {
        if (Quantity >= 4 && Quantity <= 9)
        {
            Discount = (Quantity * UnitPrice) * 0.10m;
        }
        else if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = (Quantity * UnitPrice) * 0.20m;
        }
        else
        {
            Discount = 0;
        }

        CalculateTotal();
    }
}