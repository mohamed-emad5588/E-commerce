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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            var responseData = new
            {
                totalCount = dtos.Count(),
                categories = dtos
            };

            return Ok(new ApiResponse<object>(responseData, "Categories fetched with product counts"));
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<string>(null, "Category not found", "error"));

            var dto = _mapper.Map<CategoryDto>(category);
            return Ok(new ApiResponse<CategoryDto>(dto, "Category found"));
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _categoryRepository.AddAsync(category);

            var resultDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },
                new ApiResponse<CategoryDto>(resultDto, "Category created successfully"));
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new ApiResponse<string>(null, "Category not found", "error"));

            _mapper.Map(dto, existing);
            await _categoryRepository.UpdateAsync(existing);
            return Ok(new ApiResponse<string>(null, "Category updated successfully"));
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _categoryRepository.ExistsAsync(id);
            if (!exists)
                return NotFound(new ApiResponse<string>(null, "Category not found", "error"));

            await _categoryRepository.DeleteAsync(id);
            return Ok(new ApiResponse<string>(null, "Category deleted successfully"));
        }
    }
}
