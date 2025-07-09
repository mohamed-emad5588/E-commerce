using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using E_commerce.DTOs;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using E_commerce.Services;
using E_commerce.Wrappers;
using System.Security.Cryptography;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthController(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new ApiResponse<string>(null, "Email already in use", "error"));

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
                return BadRequest(new ApiResponse<string>(null, "Password must be at least 8 characters long", "error"));

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var confirmationCode = GenerateConfirmationCode();

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = dto.Role,
                EmailConfirmationCode = confirmationCode,
                CodeGeneratedAt = DateTime.UtcNow,
                IsEmailConfirmed = false
            };

            await _userRepository.AddAsync(user);

            var body = $@"
                <h2>Email Confirmation</h2>
                <p>Your confirmation code is:</p>
                <h3 style='color:blue'>{confirmationCode}</h3>
                <p>This code will expire in 10 minutes.</p>";

            await _emailService.SendEmailAsync(user.Email, "Email Confirmation", body);

            return Ok(new ApiResponse<string>(null, "User registered successfully. Please check your email for confirmation code."));
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new ApiResponse<string>(null, "User not found", "error"));

            if (user.IsEmailConfirmed)
                return BadRequest(new ApiResponse<string>(null, "Email already confirmed", "error"));

            if (user.CodeGeneratedAt == null || DateTime.UtcNow - user.CodeGeneratedAt > TimeSpan.FromMinutes(10))
                return BadRequest(new ApiResponse<string>(null, "Confirmation code expired. Please register again or request a new code.", "error"));

            if (user.EmailConfirmationCode != dto.Code)
                return BadRequest(new ApiResponse<string>(null, "Invalid confirmation code", "error"));

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.CodeGeneratedAt = null;

            await _userRepository.UpdateAsync(user);

            var token = _tokenService.CreateToken(user);
            var result = new
            {
                token,
                user = new { user.Id, user.Name, user.Email, Role = user.Role.ToString() }
            };

            return Ok(new ApiResponse<object>(result, "Email confirmed and login successful"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new ApiResponse<string>(null, "Invalid email or password", "error"));

            if (!user.IsEmailConfirmed)
                return Unauthorized(new ApiResponse<string>(null, "Please confirm your email before logging in", "error"));

            var token = _tokenService.CreateToken(user);

            var result = new
            {
                token,
                user = new { user.Id, user.Name, user.Email, Role = user.Role.ToString() }
            };

            return Ok(new ApiResponse<object>(result, "Login successful"));
        }

        [HttpPost("resend-confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendCodeDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new ApiResponse<string>(null, "User not found", "error"));

            if (user.IsEmailConfirmed)
                return BadRequest(new ApiResponse<string>(null, "Email is already confirmed", "error"));

            var newCode = GenerateConfirmationCode();
            user.EmailConfirmationCode = newCode;
            user.CodeGeneratedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var body = $@"
                <h2>Resend Confirmation Code</h2>
                <p>Your new confirmation code is:</p>
                <h3 style='color:green'>{newCode}</h3>
                <p>This code will expire in 10 minutes.</p>";

            await _emailService.SendEmailAsync(user.Email, "Resend Confirmation Code", body);

            return Ok(new ApiResponse<string>(null, "New confirmation code sent"));
        }

        // ✅ method to generate a secure random code
        private string GenerateConfirmationCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }
    }
}
