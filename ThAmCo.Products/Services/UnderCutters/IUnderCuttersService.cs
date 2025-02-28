using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThAmCo.Products.Services.UnderCutters
{
    public interface IUnderCuttersService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        
        // ✅ Ensure this method exists in the interface
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchText);
    }
}
