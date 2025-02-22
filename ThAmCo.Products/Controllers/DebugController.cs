using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Data.Products;
using ThAmCo.Products.Services.ProductsRepo;
using ThAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly  IProductsRepo _productRepository;

    public DebugController(ILogger<DebugController> logger,
                           IUnderCuttersService underCuttersService,
                           IProductsRepo productsRepo)
    {
        _logger = logger;
        _underCuttersService = underCuttersService;
        _productRepository = productsRepo;
    }

    // GET: /debug/undercutters
    [HttpGet("UnderCutters")]
    public async Task<IActionResult> UnderCutters()
    {
        IEnumerable<ProductDto> products = null!;
        try
        {
            products = await _underCuttersService.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using UnderCutters service.");
            products = Array.Empty<ProductDto>();
        }
        return Ok(products.ToList());
    }

    // GET: /debug/repo
    [HttpGet("repo")]
    public async Task<IActionResult> Repo()
    {
        IEnumerable<ThAmCo.Products.Services.ProductsRepo.Product> products = null;
        try
        {
            products = await _productRepository.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using Products repo.");
            products = Array.Empty<ThAmCo.Products.Services.ProductsRepo.Product>();
        }

        return Ok(products.ToList());
    }}
