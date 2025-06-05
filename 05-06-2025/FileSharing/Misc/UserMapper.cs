namespace FileApp.Misc;

using FileApp.Models;
using FileApp.Models.DTOs;

public class UserMapper
{
    public User UserRequestDtoToUser(UserRequestDto userRequestDto)
    {
        return new User
        {
            UserName = userRequestDto.UserName,
            Email = userRequestDto.Email,
            Password = System.Text.Encoding.UTF8.GetBytes(userRequestDto.Password),
            Role = userRequestDto.Role,
            CreatedAt = DateTime.UtcNow
        };
    }
}