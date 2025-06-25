using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id);
    Task<Sale?> GetBySaleNumberAsync(string saleNumber);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<IEnumerable<Sale>> GetBySpecificationAsync(ISpecification<Sale> specification);
    Task<Sale> AddAsync(Sale sale);
    Task<Sale> UpdateAsync(Sale sale);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsBySaleNumberAsync(string saleNumber);
}