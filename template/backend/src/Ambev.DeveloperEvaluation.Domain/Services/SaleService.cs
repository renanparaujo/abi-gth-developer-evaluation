using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<string> GenerateSaleNumberAsync()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"SALE-{timestamp}-{random}";
    }

    public async Task<bool> ValidateSaleItemsAsync(IEnumerable<SaleItem> items)
    {
        foreach (var item in items)
        {
            if (item.Quantity > 20)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<decimal> CalculateTotalAmountAsync(IEnumerable<SaleItem> items)
    {
        decimal total = 0;

        foreach (var item in items)
        {
            item.ApplyDiscount();
            total += item.TotalAmount;
        }

        return total;
    }
}