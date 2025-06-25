using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleService, SaleService>();

        var provider = builder.Configuration["DatabaseProvider"] ?? "PostgreSQL";
        if (provider == "MongoDB")
        {
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var conn = builder.Configuration.GetConnectionString("MongoDb");
                return new MongoClient(conn);
            });
            builder.Services.AddScoped<ISaleRepository, SaleMongoRepository>();
        }
        else
        {
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        }
    }
}