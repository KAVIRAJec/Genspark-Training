using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class FreelancerService : IFreelancerService
{
    private readonly IRepository<Guid, Freelancer> _freelancerRepository;
    private readonly IRepository<string, User> _userRepository;
    private readonly FreelanceDBContext _appContext;
    private readonly IGetOrCreateSkills _getOrCreateSkills;
    private readonly IImageUploadService _imageUploadService;
    public FreelancerService(IRepository<Guid, Freelancer> freelancerRepository,
                             IRepository<string, User> userRepository,
                             FreelanceDBContext appContext,
                             IGetOrCreateSkills getOrCreateSkills,
                             IImageUploadService imageUploadService)
    {
        _freelancerRepository = freelancerRepository;
        _userRepository = userRepository;
        _appContext = appContext;
        _getOrCreateSkills = getOrCreateSkills;
        _imageUploadService = imageUploadService;
    }

    public virtual async Task<FreelancerResponseDTO> CreateFreelancer(CreateFreelancerDTO createDto)
    {
        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            var user = await UserMapper.CreateUserFromCreateFreelancerDTO(createDto);
            var existing = await _appContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (existing != null) throw new AppException("User with this email already exist", 400);
            var newUser = await _userRepository.Add(user);
            if (newUser == null) throw new AppException("Unable to create user.", 500);

            var requiredSkills = new List<Skill>();
            if (createDto.Skills != null && createDto.Skills.Count() > 0)
                requiredSkills = await _getOrCreateSkills.GetOrCreateSkills(createDto.Skills);

            var freelancer = FreelancerMapper.CreateFreelancerFromCreateDTO(createDto, requiredSkills);
            freelancer.Email = newUser.Email;

            var response = await _freelancerRepository.Add(freelancer);
            if (response == null) throw new AppException("Unable to create freelancer.", 500);

            await transaction.CommitAsync();
            return FreelancerMapper.ToResponseDTO(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new AppException($"Error creating freelancer: {inner}", 500);
        }
    }

    public virtual async Task<FreelancerResponseDTO> DeleteFreelancer(Guid freelancerId)
    {
        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            if (freelancerId == Guid.Empty) throw new AppException("Freelancer ID is required.", 400);
            var freelancer = await _freelancerRepository.Get(freelancerId);
            if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);
            if (freelancer.ProfileUrl != null) await _imageUploadService.DeleteImage(freelancer.ProfileUrl);

            var response = await _freelancerRepository.Delete(freelancerId);
            if (response == null) throw new AppException("Unable to delete freelancer.", 500);

            var user = await _userRepository.Get(freelancer.Email);
            if (user == null || user.IsActive == false) throw new AppException("User not found/ inactive.", 404);
            var userResponse = await _userRepository.Delete(user.Email);
            if (userResponse == null) throw new AppException("Unable to delete user.", 500);

            await transaction.CommitAsync();
            return FreelancerMapper.ToResponseDTO(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new AppException($"Error deleting freelancer: {inner}", 500);
        }
    }

    public async Task<PagedResponse<FreelancerResponseDTO>> GetAllFreelancersPaged(PaginationParams paginationParams)
    {
        var query = _appContext.Freelancers
            .Where(f => f.IsActive == true)
            .Include(f => f.Skills)
            .Select(f => f);

        if (!string.IsNullOrEmpty(paginationParams.Search))
            query = query.Where(f => f.Username.Contains(paginationParams.Search)
                                || f.Email.Contains(paginationParams.Search)
                                || f.Location.Contains(paginationParams.Search)
                                || f.Skills.Any(s => s.Name.Contains(paginationParams.Search)));

        if (!string.IsNullOrEmpty(paginationParams.SortBy))
        {
            switch (paginationParams.SortBy.ToLower())
            {
                case "hourlyrate":
                    query = query.OrderByDescending(f => f.HourlyRate);
                    break;
                case "location":
                    query = query.OrderByDescending(f => f.Location == paginationParams.SortBy);
                    break;
                default:
                    query = query.OrderByDescending(f => f.CreatedAt);
                    break;
            }
        }
        else
            query = query.OrderByDescending(f => f.CreatedAt);

        var result = query.Select(f => FreelancerMapper.ToResponseDTO(f));

        return await result.ToPagedResponse(paginationParams);
    }

    public async Task<FreelancerResponseDTO> GetFreelancerById(Guid freelancerId)
    {
        if (freelancerId == Guid.Empty) throw new AppException("Freelancer ID is required.", 400);
        var freelancer = await _freelancerRepository.Get(freelancerId);
        if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);
        return FreelancerMapper.ToResponseDTO(freelancer);
    }

    public async Task<FreelancerResponseDTO> UpdateFreelancer(Guid freelancerId, UpdateFreelancerDTO updateDto)
    {
        if (freelancerId == Guid.Empty) throw new AppException("Freelancer ID is required.", 400);
        if (updateDto == null) throw new AppException("Freelancer DTO is required.", 400);

        var freelancer = await _freelancerRepository.Get(freelancerId);
        if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);
        if (updateDto.ProfileUrl != null && freelancer.ProfileUrl != null) await _imageUploadService.DeleteImage(freelancer.ProfileUrl);

        var skills = new List<Skill>();
            if (updateDto.Skills != null && updateDto.Skills.Count() > 0)
                skills = await _getOrCreateSkills.GetOrCreateSkills(updateDto.Skills);

        freelancer.ProfileUrl = updateDto.ProfileUrl ?? freelancer.ProfileUrl;
        freelancer.Username = updateDto.Username ?? freelancer.Username;
        freelancer.ExperienceYears = updateDto.ExperienceYears ?? freelancer.ExperienceYears;
        freelancer.HourlyRate = updateDto.HourlyRate ?? freelancer.HourlyRate;
        freelancer.Location = updateDto.Location ?? freelancer.Location;
        freelancer.UpdatedAt = DateTime.UtcNow;
        freelancer.Skills = skills.Count > 0 ? skills : freelancer.Skills;

        var updatedFreelancer = await _freelancerRepository.Update(freelancerId, freelancer);
        if (updatedFreelancer == null) throw new AppException("Unable to update freelancer.", 500);

        return FreelancerMapper.ToResponseDTO(updatedFreelancer);
    }
}