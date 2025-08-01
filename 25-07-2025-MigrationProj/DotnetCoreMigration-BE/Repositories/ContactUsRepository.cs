using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class ContactUsRepository : IRepository<ContactUs, int>
{
    private readonly ApplicationDbContext _context;

    public ContactUsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactUs>> GetAll()
    {
        return await _context.ContactUs
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<ContactUs>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.ContactUs
            .Where(c => c.IsActive)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => 
                c.Name.ToLower().Contains(request.SearchTerm.ToLower()) ||
                c.Email.ToLower().Contains(request.SearchTerm.ToLower()) ||
                (c.Subject != null && c.Subject.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (c.Message != null && c.Message.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "name":
                    query = request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                    break;
                case "email":
                    query = request.SortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email);
                    break;
                case "createddate":
                    query = request.SortDescending ? query.OrderByDescending(c => c.CreatedDate) : query.OrderBy(c => c.CreatedDate);
                    break;
                default:
                    query = query.OrderByDescending(c => c.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedDate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<ContactUs>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<ContactUs?> GetById(int id)
    {
        return await _context.ContactUs
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(c => c.ContactId == id);
    }

    public async Task<ContactUs> Create(ContactUs contactUs)
    {
        _context.ContactUs.Add(contactUs);
        await _context.SaveChangesAsync();
        return contactUs;
    }

    public async Task<ContactUs> Update(ContactUs contactUs)
    {
        _context.ContactUs.Update(contactUs);
        await _context.SaveChangesAsync();
        return contactUs;
    }

    public async Task<bool> Delete(int id)
    {
        var contactUs = await _context.ContactUs.FindAsync(id);
        if (contactUs == null) return false;

        // Soft delete - set IsActive to false
        contactUs.IsActive = false;
        _context.ContactUs.Update(contactUs);
        await _context.SaveChangesAsync();
        return true;
    }
}
