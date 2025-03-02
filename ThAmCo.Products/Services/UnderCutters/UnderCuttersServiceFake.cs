using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Products.Services.UnderCutters
{
    public class UnderCuttersServiceFake : IUnderCuttersService
    {
        private readonly ProductDto[] _products =
        {
            new ProductDto { Id = 1, Name = "Fake product A", Description = "Red", Quantity = 1 },
            new ProductDto { Id = 2, Name = "Fake product B", Description = "Green", Quantity = 3 },
            new ProductDto { Id = 3, Name = "Fake product C", Description = "Yellow", Quantity = 5 }
        };

        public Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = _products.AsEnumerable();
            return Task.FromResult(products);
        }

        // ✅ Updated Search to Include Description
        public Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchText)
        {
            var matchingProducts = _products.Where(p =>
                p.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ||
                p.Description.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) // ✅ Search by color
            ).ToList();

            return Task.FromResult((IEnumerable<ProductDto>)matchingProducts);
        }
    }
}
