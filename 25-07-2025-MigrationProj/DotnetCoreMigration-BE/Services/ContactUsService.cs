using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;

namespace DotnetCoreMigration.Services;

public class ContactUsService : IContactUsService
{
    private readonly IRepository<ContactUs, int> _contactUsRepository;

    public ContactUsService(IRepository<ContactUs, int> contactUsRepository)
    {
        _contactUsRepository = contactUsRepository;
    }

    public async Task<IEnumerable<ContactUsDto>> GetAllContactUsAsync()
    {
        var contactUs = await _contactUsRepository.GetAll();
        return ContactUsMapper.ToDtoList(contactUs);
    }

    public async Task<PaginatedResponse<ContactUsDto>> GetAllContactUsPaginatedAsync(PaginationRequest request)
    {
        var paginatedContactUs = await _contactUsRepository.GetAllPaginated(request);
        var contactUsDtos = ContactUsMapper.ToDtoList(paginatedContactUs.Data);

        return new PaginatedResponse<ContactUsDto>(
            contactUsDtos.ToList(),
            paginatedContactUs.TotalCount,
            paginatedContactUs.PageNumber,
            paginatedContactUs.PageSize
        );
    }

    public async Task<ContactUsDto?> GetContactUsByIdAsync(int id)
    {
        var contactUs = await _contactUsRepository.GetById(id);
        return contactUs == null ? null : ContactUsMapper.ToDto(contactUs);
    }

    public async Task<IEnumerable<ContactUsDto>> GetContactUsByEmailAsync(string email)
    {
        var allContactUs = await _contactUsRepository.GetAll();
        var userContactUs = allContactUs.Where(c => c.Email.ToLower() == email.ToLower());
        return ContactUsMapper.ToDtoList(userContactUs);
    }

    public async Task<ContactUsDto> CreateContactUsAsync(CreateContactUsDto createContactUsDto)
    {
        var contactUs = ContactUsMapper.ToEntity(createContactUsDto);
        var createdContactUs = await _contactUsRepository.Create(contactUs);
        return ContactUsMapper.ToDto(createdContactUs);
    }

    public async Task<ContactUsDto> UpdateContactUsAsync(UpdateContactUsDto updateContactUsDto)
    {
        var existingContactUs = await _contactUsRepository.GetById(updateContactUsDto.ContactId);
        if (existingContactUs == null)
        {
            throw new InvalidOperationException("ContactUs not found.");
        }

        ContactUsMapper.UpdateEntity(existingContactUs, updateContactUsDto);
        var updatedContactUs = await _contactUsRepository.Update(existingContactUs);
        return ContactUsMapper.ToDto(updatedContactUs);
    }

    public async Task<bool> DeleteContactUsAsync(int id)
    {
        return await _contactUsRepository.Delete(id);
    }

    public async Task<bool> ContactUsExistsAsync(int id)
    {
        var contactUs = await _contactUsRepository.GetById(id);
        return contactUs != null;
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var contactUs = await _contactUsRepository.GetById(id);
        if (contactUs == null) return false;

        contactUs.IsRead = true;
        await _contactUsRepository.Update(contactUs);
        return true;
    }

    public async Task<IEnumerable<ContactUsDto>> GetUnreadContactUsAsync()
    {
        var allContactUs = await _contactUsRepository.GetAll();
        var unreadContactUs = allContactUs.Where(c => !c.IsRead);
        return ContactUsMapper.ToDtoList(unreadContactUs);
    }
}
