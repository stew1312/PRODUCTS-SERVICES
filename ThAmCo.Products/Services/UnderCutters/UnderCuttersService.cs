using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ThAmCo.Products.Services.UnderCutters
{
    public class UnderCuttersService : IUnderCuttersService
    {
        private readonly HttpClient _httpClient;

        public UnderCuttersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("https://api.undercutters.com/products")
                   ?? Array.Empty<ProductDto>();
        }

        // ✅ Implement missing SearchProductsAsync method
        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchText)
        {
            var allProducts = await GetProductsAsync();

            var matchingProducts = allProducts.Where(p =>
                p.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
            ).ToList();

            return matchingProducts;
        }
    }
}
