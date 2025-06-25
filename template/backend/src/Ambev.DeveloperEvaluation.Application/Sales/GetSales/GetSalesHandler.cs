using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Ambev.DeveloperEvaluation.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<GetSalesResult> Handle(GetSalesCommand request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync();

        var filteredSales = sales.AsEnumerable();

        if (!string.IsNullOrEmpty(request.CustomerName))
        {
            filteredSales = filteredSales.Where(s => s.CustomerName.Contains(request.CustomerName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.BranchName))
        {
            filteredSales = filteredSales.Where(s => s.BranchName.Contains(request.BranchName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            filteredSales = filteredSales.Where(s => s.Status.ToString().Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinDate.HasValue)
        {
            filteredSales = filteredSales.Where(s => s.SaleDate >= request.MinDate.Value);
        }

        if (request.MaxDate.HasValue)
        {
            filteredSales = filteredSales.Where(s => s.SaleDate <= request.MaxDate.Value);
        }

        if (request.MinAmount.HasValue)
        {
            filteredSales = filteredSales.Where(s => s.TotalAmount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            filteredSales = filteredSales.Where(s => s.TotalAmount <= request.MaxAmount.Value);
        }

        var totalCount = filteredSales.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.Size);
        var skip = (request.Page - 1) * request.Size;

        var pagedSales = filteredSales
            .Skip(skip)
            .Take(request.Size)
            .ToList();

        return new GetSalesResult
        {
            Items = pagedSales.Select(sale => new GetSaleSummaryResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName,
                TotalAmount = sale.TotalAmount,
                BranchName = sale.BranchName,
                Status = sale.Status.ToString(),
                ItemsCount = sale.Items.Count
            }).ToList(),
            Pagination = new PaginatedResponse
            {
                Page = request.Page,
                Size = request.Size,
                TotalCount = totalCount,
                TotalPages = totalPages
            }
        };
    }
}