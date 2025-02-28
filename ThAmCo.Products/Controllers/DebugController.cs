using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Services.ProductsRepo;
using ThAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly IProductsRepo _productRepository;

    public DebugController(ILogger<DebugController> logger,
                           IUnderCuttersService underCuttersService,
                           IProductsRepo productsRepo)
    {
        _logger = logger;
        _underCuttersService = underCuttersService;
        _productRepository = productsRepo;
    }

    // ✅ GET: /debug/undercutters - Fetch All UnderCutters Products
    [HttpGet("undercutters")]
    public async Task<IActionResult> UnderCutters()
    {
        IEnumerable<ProductDto> products;
        try
        {
            products = await _underCuttersService.GetProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"❌ Exception occurred in UnderCutters service: {ex.Message}");
            products = Array.Empty<ProductDto>();
        }
        return Ok(products.ToList());
    }

    // ✅ GET: /debug/undercutters/search?name=ProductA&id=1 - Search UnderCutters by Name or ID
    [HttpGet("undercutters/search")]
    public async Task<IActionResult> SearchUnderCutters([FromQuery] string? name = null, [FromQuery] int? id = null)
    {
        if (string.IsNullOrEmpty(name) && id == null)
        {
            return BadRequest("❌ Please provide either an ID or Name for search.");
        }

        IEnumerable<ProductDto> products;
        try
        {
            products = await _underCuttersService.GetProductsAsync();

            // ✅ Apply search filter if name or ID is provided
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            }
            if (id.HasValue)
            {
                products = products.Where(p => p.Id == id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"❌ Exception occurred searching UnderCutters products: {ex.Message}");
            products = Array.Empty<ProductDto>();
        }

        if (!products.Any())
        {
            return NotFound("❌ No UnderCutters products found matching the criteria.");
        }

        return Ok(products);
    }

    // ✅ GET: /debug/repo - Fetch All Products from ProductsRepo
    [HttpGet("repo")]
    public async Task<IActionResult> Repo([FromQuery] string? name = null, [FromQuery] int? id = null)
    {
        IEnumerable<ThAmCo.Products.Services.ProductsRepo.Product> products;
        try
        {
            products = await _productRepository.GetProductsAsync();

            // ✅ Apply search filter if name or ID is provided
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            }
            if (id.HasValue)
            {
                products = products.Where(p => p.Id == id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"❌ Exception occurred using Products repo: {ex.Message}");
            products = Array.Empty<ThAmCo.Products.Services.ProductsRepo.Product>();
        }

        return Ok(products.ToList());
    }
}
