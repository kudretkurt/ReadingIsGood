using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ReadingIsGood.ProductService.Context;
using ReadingIsGood.ProductService.Repositories;

namespace ProductService.Tests
{
    public class TestFixture
    {
        private readonly ServiceProvider _serviceProvider;

        public TestFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ProductServiceDbContext>(options =>
                options.UseInMemoryDatabase("inMemoryDbInstance")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            serviceCollection.AddScoped<IProductRepository>(provider =>
                new ProductRepository(provider.GetRequiredService<ProductServiceDbContext>()));

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IProductRepository ProductRepository => _serviceProvider.GetRequiredService<IProductRepository>();
    }
}