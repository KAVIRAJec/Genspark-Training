using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Address = user.Address,
            CreatedDate = user.CreatedDate,
            IsActive = user.IsActive,
            Role = user.Role,
            OrderCount = user.Orders?.Count(o => o.IsActive) ?? 0,
            ProductCount = user.Products?.Count(p => p.IsActive) ?? 0,
            NewsCount = user.News?.Count(n => n.IsActive) ?? 0
        };
    }

    public static User ToEntity(CreateUserDto createDto)
    {
        return new User
        {
            UserName = createDto.UserName,
            Email = createDto.Email,
            Password = createDto.Password, // This should be hashed in the service
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Phone = createDto.Phone,
            Address = createDto.Address,
            Role = createDto.Role ?? "User",
            IsActive = true
        };
    }

    public static void UpdateEntity(User entity, UpdateUserDto updateDto)
    {
        entity.UserName = updateDto.UserName;
        entity.Email = updateDto.Email;
        entity.FirstName = updateDto.FirstName;
        entity.LastName = updateDto.LastName;
        entity.Phone = updateDto.Phone;
        entity.Address = updateDto.Address;
        if (!string.IsNullOrEmpty(updateDto.Role))
        {
            entity.Role = updateDto.Role;
        }
    }

    public static void UpdateProfileEntity(User entity, UpdateUserProfileDto updateDto)
    {
        entity.FirstName = updateDto.FirstName;
        entity.LastName = updateDto.LastName;
        entity.Phone = updateDto.Phone;
        entity.Address = updateDto.Address;
    }

    public static IEnumerable<UserDto> ToDtoList(IEnumerable<User> users)
    {
        return users.Select(ToDto);
    }
}
