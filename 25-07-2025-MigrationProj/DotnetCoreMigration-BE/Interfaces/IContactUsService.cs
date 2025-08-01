using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IContactUsService
{
    Task<IEnumerable<ContactUsDto>> GetAllContactUsAsync();
    Task<PaginatedResponse<ContactUsDto>> GetAllContactUsPaginatedAsync(PaginationRequest request);
    Task<ContactUsDto?> GetContactUsByIdAsync(int id);
    Task<IEnumerable<ContactUsDto>> GetContactUsByEmailAsync(string email);
    Task<ContactUsDto> CreateContactUsAsync(CreateContactUsDto createContactUsDto);
    Task<ContactUsDto> UpdateContactUsAsync(UpdateContactUsDto updateContactUsDto);
    Task<bool> DeleteContactUsAsync(int id);
    Task<bool> ContactUsExistsAsync(int id);
    Task<bool> MarkAsReadAsync(int id);
    Task<IEnumerable<ContactUsDto>> GetUnreadContactUsAsync();
}
