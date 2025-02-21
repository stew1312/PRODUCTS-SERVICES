using System;

namespace ThAmCo.Products.Services.UnderCutters;

public interface IUnderCuttersService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
