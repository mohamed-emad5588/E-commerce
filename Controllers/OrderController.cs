using System.Security.Claims;
using AutoMapper;
using E_commerce.DTOs;
using E_commerce.Models;
using E_commerce.Repositories.Implementations;
using E_commerce.Repositories.Interfaces;
using E_commerce.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static E_commerce.Models.Order;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
       

        public OrdersController(
                      IOrderRepository orderRepository,
                      IMapper mapper, 
                      ICartItemRepository cartItemRepository)
        {
            _orderRepository = orderRepository;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(new ApiResponse<IEnumerable<OrderDto>>(dtos, "Orders fetched successfully"));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound(new ApiResponse<string>(null!, "Order not found", "error"));

            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

   
            if (User.IsInRole("User") && order.UserId != userIdFromToken)
                return Forbid();

            var dto = _mapper.Map<OrderDto>(order);
            return Ok(new ApiResponse<OrderDto>(dto, "Order found"));
        }


        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId != userIdFromToken && !User.IsInRole("Admin"))
                return Forbid();
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(new ApiResponse<IEnumerable<OrderDto>>(dtos, "User's orders fetched"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var order = _mapper.Map<Order>(dto);

            // إعداد التاريخ يدويًا
            order.OrderDate = DateTime.Now;

            await _orderRepository.AddAsync(order);

            var resultDto = _mapper.Map<OrderDto>(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, new ApiResponse<OrderDto>(resultDto, "Order created successfully"));
        }
        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new ApiResponse<string>(null, "Invalid user token", "error"));

            int userId = int.Parse(userIdClaim.Value);

            var cartItems = await _cartItemRepository.GetByUserIdAsync(userId);
            if (!cartItems.Any())
                return BadRequest(new ApiResponse<string>(null, "Cart is empty", "error"));

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList()
            };

            
            await _orderRepository.AddAsync(order);

            
            foreach (var item in cartItems)
            {
                await _cartItemRepository.DeleteAsync(item.Id);
            }

            return Ok(new ApiResponse<object>(new
            {
                orderId = order.Id,
                totalAmount = order.TotalAmount,
                itemCount = order.OrderItems.Count
            }, "Order placed successfully"));
        }

        [HttpGet("invoice/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoice(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return NotFound(new ApiResponse<string>(null, "Order not found", "error"));

            var userIdFromToken = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (User.IsInRole("User") && order.UserId != userIdFromToken)
                return Forbid();

            var invoice = _mapper.Map<OrderInvoiceDto>(order);
            return Ok(new ApiResponse<OrderInvoiceDto>(invoice, "Invoice retrieved successfully"));
        }


        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound(new ApiResponse<string>(null, "Order not found", "error"));

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                return BadRequest(new ApiResponse<string>(null, "Invalid status value", "error"));

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);

            return Ok(new ApiResponse<string>($"Order status updated to {newStatus}"));
        }


    }
}


