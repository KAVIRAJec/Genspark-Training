using FileApp.Models;

namespace FileApp.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}