using E_commerce.Models;

namespace E_commerce.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}

