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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ReviewsController(IReviewRepository reviewRepository, IMapper mapper, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            var dtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(new ApiResponse<IEnumerable<ReviewDto>>(dtos, "Reviews fetched"));
        }

        
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new ApiResponse<string>(null, "Invalid user token", "error"));

            int userId = int.Parse(userIdClaim.Value);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var review = _mapper.Map<Review>(dto);
            review.UserId = user.Id;

            await _reviewRepository.AddAsync(review);
            return Ok(new ApiResponse<string>(null, "Review added"));
        }
    }
}
