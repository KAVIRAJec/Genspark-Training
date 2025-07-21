using VideoStreamingPlatform.Models;

namespace VideoStreamingPlatform.Interfaces;

public interface ITrainingVideoRepository
{
    Task<IEnumerable<TrainingVideo>> GetAllAsync();
    Task<TrainingVideo?> GetByIdAsync(int id);
    Task<TrainingVideo> CreateAsync(TrainingVideo video);
    Task<TrainingVideo> UpdateAsync(TrainingVideo video);
    Task<bool> DeleteAsync(int id);
    Task<(IEnumerable<TrainingVideo> Videos, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? sortBy = null, 
        bool sortDescending = true);
    Task<bool> ExistsAsync(int id);
}
