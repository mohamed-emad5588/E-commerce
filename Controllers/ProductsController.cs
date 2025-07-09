using AutoMapper;
using E_commerce.DTOs;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using E_commerce.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var responseData = new
            {
                totalCount = productDtos.Count(),
                totalStock = productDtos.Sum(p => p.Stock),
                products = productDtos
            };

            return Ok(new ApiResponse<object>(responseData, "Products fetched with statistics"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse<string>(null, "Product not found", "error"));

            var dto = _mapper.Map<ProductDto>(product);
            return Ok(new ApiResponse<ProductDto>(dto, "Product found"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            if (dto.Image != null)
            {
                var folderPath = Path.Combine("wwwroot", "images", "products");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                product.ImageUrl = $"/images/products/{fileName}";
            }

            await _productRepository.AddAsync(product);

            var resultDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, new ApiResponse<ProductDto>(resultDto, "Product created with image"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateProductDto dto)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new ApiResponse<string>(null, "Product not found", "error"));

            _mapper.Map(dto, existing);
            await _productRepository.UpdateAsync(existing);
            return Ok(new ApiResponse<string>(null, "Product updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
                return NotFound(new ApiResponse<string>(null, "Product not found", "error"));

            await _productRepository.DeleteAsync(id);
            return Ok(new ApiResponse<string>(null, "Product deleted successfully"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var (products, totalCount) = await _productRepository.GetFilteredPaginatedAsync(
                name, categoryId, minPrice, maxPrice, pageNumber, pageSize);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var result = new
            {
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                items = productDtos
            };

            return Ok(new ApiResponse<object>(result, "Filtered products fetched successfully"));
        }
    }
}
