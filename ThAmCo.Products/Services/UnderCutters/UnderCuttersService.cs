using System;
using System.Net;
using ThAmCo.Products.Services.UnderCutters;

namespace ThAnCo.Products.Services.UnderCutters;

public class UnderCuttersService : IUnderCuttersService
{
    private readonly HttpClient _client;

    public UnderCuttersService(HttpClient client, IConfiguration configuration)
    {
        var baseUrl = configuration["WebServices:UnderCutters:BaseURL"] ?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client = client;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var uri = "api/product";
        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var reviews = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

        return reviews;
    }
}
