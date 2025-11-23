using Assignment.Dtos.Products;
using Assignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService) { _productService = productService; }

        [HttpGet]
        public async Task<ActionResult<object>> GetProducts([FromQuery] string? search, [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var items = await _productService.GetProductsAsync(search, categoryId, minPrice, maxPrice);
            return Ok(new { items });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(Guid id)
        {
            var p = await _productService.GetProductAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CombosController : ControllerBase
    {
        private readonly IComboService _comboService;
        public CombosController(IComboService comboService) { _comboService = comboService; }

        [HttpGet]
        public async Task<ActionResult<object>> GetCombos([FromQuery] string? search)
        {
            var items = await _comboService.GetCombosAsync(search);
            return Ok(new { items });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ComboDetailDto>> GetCombo(Guid id)
        {
            var combo = await _comboService.GetComboAsync(id);
            if (combo == null) return NotFound();
            return Ok(combo);
        }
    }
}
