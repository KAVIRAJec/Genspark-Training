using System.Transactions;
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class ClientService : IClientService
{
    private readonly IRepository<Guid, Client> _clientRepository;
    private readonly IRepository<string, User> _userRepository;
    private readonly FreelanceDBContext _appContext;
    private readonly IImageUploadService _imageUploadService;
    public ClientService(IRepository<Guid, Client> clientRepository,
                        IRepository<string, User> userRepository,
                        FreelanceDBContext appContext,
                        IImageUploadService imageUploadService)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _appContext = appContext;
        _imageUploadService = imageUploadService;
    }

    public virtual async Task<ClientResponseDTO> CreateClient(CreateClientDTO createClientDTO)
    {
        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            var user = await UserMapper.CreateUserFromCreateClientDTO(createClientDTO);
            var existing = await _appContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (existing != null) throw new AppException("User with this email already exist", 400);
            var newUser = await _userRepository.Add(user);
            if (newUser == null) throw new AppException("Unable to create user.", 500);

            var client = ClientMapper.CreateClientFromCreateDTO(createClientDTO);
            client.Email = newUser.Email;

            var response = await _clientRepository.Add(client);
            if (response == null) throw new AppException("Unable to create client.", 500);

            await transaction.CommitAsync();
            return ClientMapper.ToResponseDTO(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new AppException($"Error creating client: {inner}", 500);
        }
    }

    public virtual async Task<ClientResponseDTO> DeleteClient(Guid clientId)
    {
        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
            var client = await _clientRepository.Get(clientId);
            if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);
            if (client.ProfileUrl != null) await _imageUploadService.DeleteImage(client.ProfileUrl);
            var response = await _clientRepository.Delete(clientId);
            if (response == null) throw new AppException("Unable to delete client.", 500);

            var user = await _userRepository.Get(client.Email);
            if (user == null || user.IsActive == false) throw new AppException("User not found/ inactive.", 404);
            var userResponse = await _userRepository.Delete(user.Email);
            if (userResponse == null) throw new AppException("Unable to delete user.", 500);

            await transaction.CommitAsync();
            return ClientMapper.ToResponseDTO(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new AppException($"Error deleting client: {inner}", 500);
        }
    }

    public async Task<PagedResponse<ClientResponseDTO>> GetAllClientsPaged(PaginationParams paginationParams)
    {
         var query = _appContext.Clients
        .Where(c => c.IsActive)
        .Include(c => c.Projects)
        .OrderByDescending(c => c.CreatedAt)
        .Select(c => ClientMapper.ToResponseDTO(c));

        return await query.ToPagedResponse(paginationParams);
    }

    public async Task<ClientResponseDTO> GetClientById(Guid clientId)
    {
        if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
        var client = await _clientRepository.Get(clientId);
        if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);
        return ClientMapper.ToResponseDTO(client);
    }

    public async Task<ClientResponseDTO> UpdateClient(Guid clientId, UpdateClientDTO updateClientDTO)
    {
        if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
        if (updateClientDTO == null) throw new AppException("Client DTO is required.", 400);

        var client = await _clientRepository.Get(clientId);
        if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);
        if (updateClientDTO.ProfileUrl != null && client.ProfileUrl != null) await _imageUploadService.DeleteImage(client.ProfileUrl);
        client.ProfileUrl = updateClientDTO.ProfileUrl ?? client.ProfileUrl;
        client.Username = updateClientDTO.Username ?? client.Username;
        client.CompanyName = updateClientDTO.CompanyName ?? client.CompanyName;
        client.Location = updateClientDTO.Location ?? client.Location;
        client.UpdatedAt = DateTime.UtcNow;
        
        var updatedClient = await _clientRepository.Update(clientId, client);
        if (updatedClient == null) throw new AppException("Unable to update client.", 500);

        return ClientMapper.ToResponseDTO(updatedClient);
    }
}