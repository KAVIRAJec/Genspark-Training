using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IAuthenticationService
{
    public Task<LoginResponseDTO> Login(LoginRequestDTO user);
    public Task<LoginResponseDTO> RefreshToken(string refreshToken);
    public Task<bool> Logout(string email);
    public Task<T> GetDetails<T>(string email) where T : class;
}