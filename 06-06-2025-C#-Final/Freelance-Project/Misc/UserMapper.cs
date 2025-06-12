using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Freelance_Project.Services;

namespace Freelance_Project.Misc;

public static class UserMapper
{
    public static async Task<User> CreateUserFromCreateClientDTO(CreateClientDTO createUserDTO)
    {
        if (createUserDTO == null) throw new AppException("User DTO cannot be null.", 400);

        var hash = new HashingModel
        {
            Data = createUserDTO.Password,
            HashKey = null,
            HashedData = null
        };
        var hashingService = new HashingService();
        var hashedData = await hashingService.HashData(hash);

        return new User
        {
            Email = createUserDTO.Email.ToLower(),
            Password = hashedData.HashedData,
            HashKey = hashedData.HashKey,
            Role = "Client",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    public static async Task<User> CreateUserFromCreateFreelancerDTO(CreateFreelancerDTO createUserDTO)
    {
        if (createUserDTO == null) throw new AppException("User DTO cannot be null.", 400);

        var hash = new HashingModel
        {
            Data = createUserDTO.Password,
            HashKey = null,
            HashedData = null
        };
        var hashingService = new HashingService();
        var hashedData = await hashingService.HashData(hash);

        return new User
        {
            Email = createUserDTO.Email.ToLower(),
            Password = hashedData.HashedData,
            HashKey = hashedData.HashKey,
            Role = "Freelancer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}