// Repositories/Interfaces/IStatisticsRepository.cs
using E_commerce.DTOs;

namespace E_commerce.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<WebsiteStatsDto> GetWebsiteStatsAsync();
    }

}
