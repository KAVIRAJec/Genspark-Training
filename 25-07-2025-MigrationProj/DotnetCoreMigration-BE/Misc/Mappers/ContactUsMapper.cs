using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class ContactUsMapper
{
    public static ContactUsDto ToDto(ContactUs contactUs)
    {
        return new ContactUsDto
        {
            ContactId = contactUs.ContactId,
            Name = contactUs.Name,
            Email = contactUs.Email,
            Subject = contactUs.Subject,
            Message = contactUs.Message,
            CreatedDate = contactUs.CreatedDate,
            IsRead = contactUs.IsRead,
            IsActive = contactUs.IsActive,
            Response = contactUs.Response,
            ResponseDate = contactUs.ResponseDate
        };
    }

    public static ContactUs ToEntity(CreateContactUsDto createDto)
    {
        return new ContactUs
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Subject = createDto.Subject,
            Message = createDto.Message,
            IsRead = false,
            IsActive = true
        };
    }

    public static void UpdateEntity(ContactUs entity, UpdateContactUsDto updateDto)
    {
        entity.Name = updateDto.Name;
        entity.Email = updateDto.Email;
        entity.Subject = updateDto.Subject;
        entity.Message = updateDto.Message;
        entity.IsRead = updateDto.IsRead;
        entity.Response = updateDto.Response;
        entity.ResponseDate = updateDto.ResponseDate;
    }

    public static IEnumerable<ContactUsDto> ToDtoList(IEnumerable<ContactUs> contactUsList)
    {
        return contactUsList.Select(ToDto);
    }
}
