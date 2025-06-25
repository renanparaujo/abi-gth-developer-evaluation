using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly IBus _bus;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMediator mediator,
        ILogger<CancelSaleHandler> logger,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.Id} not found");

        if (sale.Status == Domain.Enums.SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        sale.Cancel(request.Reason);

        var cancelledSale = await _saleRepository.UpdateAsync(sale);

        var saleCancelledEvent = new SaleCancelledEvent(cancelledSale);
        await _mediator.Publish(saleCancelledEvent, cancellationToken);
        await _bus.Publish(saleCancelledEvent);

        _logger.LogInformation("Sale cancelled: {SaleNumber}", cancelledSale.SaleNumber);

        return new CancelSaleResult
        {
            Id = cancelledSale.Id,
            SaleNumber = cancelledSale.SaleNumber,
            Status = cancelledSale.Status.ToString(),
            CancelledAt = cancelledSale.CancelledAt ?? DateTime.UtcNow,
            CancellationReason = cancelledSale.CancellationReason ?? string.Empty
        };
    }
}