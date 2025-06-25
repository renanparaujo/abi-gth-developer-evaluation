using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleService _saleService;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IBus _bus;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        ISaleService saleService,
        IMediator mediator,
        ILogger<CreateSaleHandler> logger,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _saleService = saleService;
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var saleNumber = await _saleService.GenerateSaleNumberAsync();

        var sale = new Sale
        {
            SaleNumber = saleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            BranchId = request.BranchId,
            BranchName = request.BranchName
        };

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

        var createdSale = await _saleRepository.AddAsync(sale);

        var saleCreatedEvent = new SaleCreatedEvent(createdSale);
        await _mediator.Publish(saleCreatedEvent, cancellationToken);
        await _bus.Publish(saleCreatedEvent);

        _logger.LogInformation("Sale created: {SaleNumber}", createdSale.SaleNumber);

        return new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            SaleDate = createdSale.SaleDate,
            CustomerId = createdSale.CustomerId,
            CustomerName = createdSale.CustomerName,
            TotalAmount = createdSale.TotalAmount,
            BranchId = createdSale.BranchId,
            BranchName = createdSale.BranchName,
            Items = createdSale.Items.Select(item => new CreateSaleItemResult
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