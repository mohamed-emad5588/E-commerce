using E_commerce.DTOs;
using E_commerce.Repositories.Interfaces;
using E_commerce.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class WebsiteController : ControllerBase
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public WebsiteController(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetWebsiteStats()
        {
            var stats = await _statisticsRepository.GetWebsiteStatsAsync();
            return Ok(new ApiResponse<WebsiteStatsDto>(stats, "Website statistics fetched"));
        }
    }
}
