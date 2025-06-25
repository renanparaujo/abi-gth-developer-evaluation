using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleMongoRepository : ISaleRepository
{
    private readonly IMongoCollection<Sale> _sales;

    public SaleMongoRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("DeveloperEvaluation");
        _sales = database.GetCollection<Sale>("Sales");
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _sales.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber)
    {
        return await _sales.Find(x => x.SaleNumber == saleNumber).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _sales.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetBySpecificationAsync(ISpecification<Sale> specification)
    {
        var all = await _sales.Find(_ => true).ToListAsync();
        return all.Where(specification.IsSatisfiedBy);
    }

    public async Task<Sale> AddAsync(Sale sale)
    {
        await _sales.InsertOneAsync(sale);
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale)
    {
        await _sales.ReplaceOneAsync(x => x.Id == sale.Id, sale);
        return sale;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _sales.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _sales.Find(x => x.Id == id).AnyAsync();
    }

    public async Task<bool> ExistsBySaleNumberAsync(string saleNumber)
    {
        return await _sales.Find(x => x.SaleNumber == saleNumber).AnyAsync();
    }
}