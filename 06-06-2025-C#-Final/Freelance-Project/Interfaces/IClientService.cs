using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IClientService
{
    public Task<ClientResponseDTO> CreateClient(CreateClientDTO createClientDTO);
    public Task<PagedResponse<ClientResponseDTO>> GetAllClientsPaged(PaginationParams paginationParams);
    public Task<ClientResponseDTO> GetClientById(Guid clientId);
    public Task<ClientResponseDTO> UpdateClient(Guid clientId, UpdateClientDTO updateClientDTO);
    public Task<ClientResponseDTO> DeleteClient(Guid clientId);

}