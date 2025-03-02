using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Products.Services.ProductsRepo
{
    public class ProductsRepoFake : IProductsRepo
    {
        private readonly Product[] _products =
        {
            new Product { Id = 1, Name = "Fake Shirt" },
            new Product { Id = 2, Name = "Fake Tie" },
            new Product { Id = 3, Name = "Fake Belt" }
        };

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = _products.AsEnumerable();
            return Task.FromResult(products);
        }

        // Searching with the ID
        public Task<Product?> SearchProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        // Searching with the name (Case-Insensitive)
        public Task<Product?> SearchProductByNameAsync(string name)
        {
            var product = _products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(product);
        }

        

    }
}
