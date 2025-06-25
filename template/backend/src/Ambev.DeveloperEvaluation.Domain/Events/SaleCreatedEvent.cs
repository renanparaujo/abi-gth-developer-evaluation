using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime SaleDate { get; }
    public Guid CustomerId { get; }
    public decimal TotalAmount { get; }

    public SaleCreatedEvent(Sale sale)
    {
        SaleId = sale.Id;
        SaleNumber = sale.SaleNumber;
        SaleDate = sale.SaleDate;
        CustomerId = sale.CustomerId;
        TotalAmount = sale.TotalAmount;
    }
}