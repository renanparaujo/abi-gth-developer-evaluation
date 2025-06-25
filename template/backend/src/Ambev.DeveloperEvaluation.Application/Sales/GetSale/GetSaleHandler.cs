using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult?>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<GetSaleResult?> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id);

        if (sale == null)
            return null;

        return new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            TotalAmount = sale.TotalAmount,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            Status = sale.Status.ToString(),
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            CancelledAt = sale.CancelledAt,
            CancellationReason = sale.CancellationReason,
            Items = sale.Items.Select(item => new GetSaleItemResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount,
                Status = item.Status.ToString(),
                CancelledAt = item.CancelledAt,
                CancellationReason = item.CancellationReason
            }).ToList()
        };
    }
}