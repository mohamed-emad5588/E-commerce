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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

      
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(new ApiResponse<IEnumerable<UserDto>>(dtos, "Users fetched successfully"));
        }

        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && id != userIdFromToken)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse<string>(null, "User not found", "error"));

            var dto = _mapper.Map<UserDto>(user);
            return Ok(new ApiResponse<UserDto>(dto, "User found"));
        }

       
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(new ApiResponse<string>(null, "Email already exists", "error"));

            var user = _mapper.Map<User>(dto);
            await _userRepository.AddAsync(user);

            var userDto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new ApiResponse<UserDto>(userDto, "User created"));
        }

        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || user.PasswordHash != dto.Password)
                return Unauthorized(new ApiResponse<string>(null, "Invalid email or password", "error"));

            var dtoOut = _mapper.Map<UserDto>(user);
            return Ok(new ApiResponse<UserDto>(dtoOut, "Login successful"));
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _userRepository.ExistsAsync(id);
            if (!exists)
                return NotFound(new ApiResponse<string>(null, "User not found", "error"));

            await _userRepository.DeleteAsync(id);
            return Ok(new ApiResponse<string>(null, "User deleted"));
        }
    }
}
