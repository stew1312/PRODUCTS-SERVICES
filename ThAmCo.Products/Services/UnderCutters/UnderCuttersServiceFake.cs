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
            new ProductDto { Id = 1, Name = "Fake product A" },
            new ProductDto { Id = 2, Name = "Fake product B" },
            new ProductDto { Id = 3, Name = "Fake product C" }
        };

        public Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = _products.AsEnumerable();
            return Task.FromResult(products);
        }

        // ✅ Implement missing SearchProductsAsync method
        public Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchText)
        {
            var matchingProducts = _products.Where(p =>
                p.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
            ).ToList();

            return Task.FromResult((IEnumerable<ProductDto>)matchingProducts);
        }
    }
}
