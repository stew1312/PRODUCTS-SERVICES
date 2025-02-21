using System;

namespace ThAmCo.Products.Services.UnderCutters;

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
}
