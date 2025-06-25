using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime ModifiedAt { get; }
    public decimal TotalAmount { get; }

    public SaleModifiedEvent(Sale sale)
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        ModifiedAt = sale.UpdatedAt ?? DateTime.UtcNow;
        TotalAmount = sale.TotalAmount;
    }
}