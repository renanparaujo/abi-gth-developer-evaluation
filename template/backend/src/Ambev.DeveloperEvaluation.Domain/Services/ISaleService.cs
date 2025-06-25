using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ISaleService
{
    Task<string> GenerateSaleNumberAsync();
    Task<bool> ValidateSaleItemsAsync(IEnumerable<SaleItem> items);
    Task<decimal> CalculateTotalAmountAsync(IEnumerable<SaleItem> items);
}