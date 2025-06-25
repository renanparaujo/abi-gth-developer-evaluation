using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IBus _bus;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMediator mediator,
        ILogger<UpdateSaleHandler> logger,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.Id} not found");

        if (sale.Status == Domain.Enums.SaleStatus.Cancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        sale.SaleDate = request.SaleDate;
        sale.CustomerId = request.CustomerId;
        sale.CustomerName = request.CustomerName;
        sale.BranchId = request.BranchId;
        sale.BranchName = request.BranchName;

        sale.Items.Clear();

        foreach (var itemCommand in request.Items)
        {
            var item = new SaleItem
            {
                SaleId = sale.Id,
                ProductId = itemCommand.ProductId,
                ProductName = itemCommand.ProductName,
                Quantity = itemCommand.Quantity,
                UnitPrice = itemCommand.UnitPrice
            };

            item.ApplyDiscount();
            sale.Items.Add(item);
        }

        sale.CalculateTotal();
        sale.Update();

        var updatedSale = await _saleRepository.UpdateAsync(sale);

        var saleModifiedEvent = new SaleModifiedEvent(updatedSale);
        await _mediator.Publish(saleModifiedEvent, cancellationToken);
        await _bus.Publish(saleModifiedEvent);

        _logger.LogInformation("Sale updated: {SaleNumber}", updatedSale.SaleNumber);

        return new UpdateSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            SaleDate = updatedSale.SaleDate,
            CustomerId = updatedSale.CustomerId,
            CustomerName = updatedSale.CustomerName,
            TotalAmount = updatedSale.TotalAmount,
            BranchId = updatedSale.BranchId,
            BranchName = updatedSale.BranchName,
            Items = updatedSale.Items.Select(item => new UpdateSaleItemResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount
            }).ToList()
        };
    }
}
