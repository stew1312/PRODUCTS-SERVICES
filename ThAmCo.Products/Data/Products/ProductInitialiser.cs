using System;

namespace ThAmCo.Products.Data.Products;

public static class ProductsInitialiser
{
    public static async Task SeedTestData(ProductsContext context)
    {
        if (context.Products.Any())
        {
            // db seems to be seeded
            return;
        }

        

        var products = new List<Product>
        {
            new() { Id = 1, Name = "Shirt" },
            new() { Id = 2, Name = "Tie" },
            new() { Id = 3, Name = "Belt" },
        };

        products.ForEach(p => context.Add(p));
        await context.SaveChangesAsync();
    }
}
