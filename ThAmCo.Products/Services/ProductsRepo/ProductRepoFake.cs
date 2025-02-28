using System;

namespace ThAmCo.Products.Services.ProductsRepo;

public class ProductsRepoFake : IProductsRepo
{
    private readonly Product[] _products =
    {
        new Product { Id = 1, Name = "Fake Shirt " },
        new Product { Id = 2, Name = "Fake Tie " },
        new Product { Id = 3, Name = "Fake Belt" }
    };

    public Task<IEnumerable<Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
