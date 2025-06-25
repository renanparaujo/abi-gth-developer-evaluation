using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

public class ActiveSaleSpecification : ISpecification<Sale>
{
    public bool IsSatisfiedBy(Sale entity)
    {
        return entity.Status == SaleStatus.Active;
    }
}