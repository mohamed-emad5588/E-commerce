using AutoMapper;
using E_commerce.DTOs;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using E_commerce.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemRepository _cartRepo;
        private readonly IMapper _mapper;

        public CartItemsController(ICartItemRepository cartRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var items = await _cartRepo.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<CartItemDto>>(items);

            var total = dtos.Sum(i => i.ProductPrice * i.Quantity);
            var result = new
            {
                count = dtos.Count(),
                total,
                items = dtos
            };

            return Ok(new ApiResponse<object>(result, "Cart items retrieved"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCartItemDto dto)
        {
            var userId = GetUserId();

            var existing = await _cartRepo.GetByUserIdAndProductIdAsync(userId, dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                await _cartRepo.UpdateAsync(existing);

                var updatedDto = _mapper.Map<CartItemDto>(existing);
                return Ok(new ApiResponse<CartItemDto>(updatedDto, "Cart item quantity updated"));
            }

            var cartItem = _mapper.Map<CartItem>(dto);
            cartItem.UserId = userId;

            await _cartRepo.AddAsync(cartItem);
            var result = _mapper.Map<CartItemDto>(cartItem);
            return Created("", new ApiResponse<CartItemDto>(result, "Item added to cart"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] int quantity)
        {
            var userId = GetUserId();
            var item = await _cartRepo.GetByIdAsync(id);

            if (item == null || item.UserId != userId)
                return NotFound(new ApiResponse<string>(null, "Cart item not found", "error"));

            if (quantity <= 0)
            {
                await _cartRepo.DeleteAsync(id);
                return Ok(new ApiResponse<string>("Item removed from cart"));
            }

            item.Quantity = quantity;
            await _cartRepo.UpdateAsync(item);

            return Ok(new ApiResponse<string>("Quantity updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var item = await _cartRepo.GetByIdAsync(id);

            if (item == null || item.UserId != userId)
                return NotFound(new ApiResponse<string>(null, "Cart item not found", "error"));

            await _cartRepo.DeleteAsync(id);
            return Ok(new ApiResponse<string>("Item removed from cart"));
        }
    }
}
