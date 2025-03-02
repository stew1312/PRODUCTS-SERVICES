using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThAmCo.Products.Services.UnderCutters
{
    public interface IUnderCuttersService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        
        
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchText);
    }
}
