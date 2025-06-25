using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new ApiResponseWithData<CreateSaleResult>
        {
            Success = true,
            Data = result,
            Message = "Sale created successfully"
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSale(Guid id)
    {
        var command = new GetSaleCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found"
            });

        return Ok(new ApiResponseWithData<GetSaleResult>
        {
            Success = true,
            Data = result,
            Message = "Sale retrieved successfully"
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetSales(
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string? _order = null,
        [FromQuery] string? customerName = null,
        [FromQuery] string? branchName = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? _minDate = null,
        [FromQuery] DateTime? _maxDate = null,
        [FromQuery] decimal? _minAmount = null,
        [FromQuery] decimal? _maxAmount = null)
    {
        var command = new GetSalesCommand
        {
            Page = _page,
            Size = _size,
            Order = _order,
            CustomerName = customerName,
            BranchName = branchName,
            Status = status,
            MinDate = _minDate,
            MaxDate = _maxDate,
            MinAmount = _minAmount,
            MaxAmount = _maxAmount
        };

        var result = await _mediator.Send(command);

        return Ok(new ApiResponseWithData<GetSalesResult>
        {
            Success = true,
            Data = result,
            Message = "Sales retrieved successfully"
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        return Ok(new ApiResponseWithData<UpdateSaleResult>
        {
            Success = true,
            Data = result,
            Message = "Sale updated successfully"
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelSale(Guid id, [FromBody] CancelSaleRequest request)
    {
        var command = new CancelSaleCommand
        {
            Id = id,
            Reason = request.Reason
        };
        var result = await _mediator.Send(command);

        return Ok(new ApiResponseWithData<CancelSaleResult>
        {
            Success = true,
            Data = result,
            Message = "Sale cancelled successfully"
        });
    }
}

public class CancelSaleRequest
{
    public string Reason { get; set; } = string.Empty;
}