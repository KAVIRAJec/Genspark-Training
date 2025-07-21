using AutoMapper;
using VideoStreamingPlatform.DTOs;
using VideoStreamingPlatform.Interfaces;
using VideoStreamingPlatform.Models;

namespace VideoStreamingPlatform.Services;

public class TrainingVideoService : ITrainingVideoService
{
    private readonly ITrainingVideoRepository _repository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<TrainingVideoService> _logger;

    public TrainingVideoService(
        ITrainingVideoRepository repository,
        IBlobStorageService blobStorageService,
        IMapper mapper,
        ILogger<TrainingVideoService> logger)
    {
        _repository = repository;
        _blobStorageService = blobStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponseDto<VideoResponseDto>> UploadVideoAsync(VideoUploadRequestDto request)
    {
        try
        {
            // Validate file
            if (!IsValidVideoFile(request.VideoFile))
            {
                return new ApiResponseDto<VideoResponseDto>
                {
                    Success = false,
                    Message = "Invalid video file",
                    Errors = new List<string> { "Only MP4, AVI, MOV, and WMV files are allowed" }
                };
            }

            // Generate unique filename
            var fileName = GenerateUniqueFileName(request.VideoFile.FileName);
            
            // Upload to blob storage
            var blobUrl = await _blobStorageService.UploadVideoAsync(request.VideoFile, fileName);
            
            // Create video entity
            var video = _mapper.Map<TrainingVideo>(request);
            video.BlobUrl = blobUrl;
            video.FileName = fileName;
            video.FileSize = request.VideoFile.Length;
            video.ContentType = request.VideoFile.ContentType;
            
            // Save to database
            var savedVideo = await _repository.CreateAsync(video);
            var responseDto = _mapper.Map<VideoResponseDto>(savedVideo);
            
            _logger.LogInformation("Video uploaded successfully with ID {VideoId}", savedVideo.Id);
            
            return new ApiResponseDto<VideoResponseDto>
            {
                Success = true,
                Message = "Video uploaded successfully",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading video: {Title}", request.Title);
            return new ApiResponseDto<VideoResponseDto>
            {
                Success = false,
                Message = "Failed to upload video",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponseDto<VideoListResponseDto>> GetVideosAsync(PaginationRequestDto pagination)
    {
        try
        {
            var (videos, totalCount) = await _repository.GetPagedAsync(
                pagination.Page,
                pagination.PageSize,
                pagination.SearchTerm,
                pagination.SortBy,
                pagination.SortDescending);

            var videoDtos = videos.Select(v => _mapper.Map<VideoResponseDto>(v)).ToList();
            
            var totalPages = (int)Math.Ceiling((double)totalCount / pagination.PageSize);
            
            var response = new VideoListResponseDto
            {
                Videos = videoDtos,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = totalPages
            };

            return new ApiResponseDto<VideoListResponseDto>
            {
                Success = true,
                Message = "Videos retrieved successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving videos");
            return new ApiResponseDto<VideoListResponseDto>
            {
                Success = false,
                Message = "Failed to retrieve videos",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponseDto<VideoResponseDto>> GetVideoByIdAsync(int id)
    {
        try
        {
            var video = await _repository.GetByIdAsync(id);
            if (video == null)
            {
                return new ApiResponseDto<VideoResponseDto>
                {
                    Success = false,
                    Message = "Video not found"
                };
            }

            var responseDto = _mapper.Map<VideoResponseDto>(video);
            
            return new ApiResponseDto<VideoResponseDto>
            {
                Success = true,
                Message = "Video retrieved successfully",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving video with ID {VideoId}", id);
            return new ApiResponseDto<VideoResponseDto>
            {
                Success = false,
                Message = "Failed to retrieve video",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponseDto<VideoStreamResponseDto>> GetVideoStreamAsync(int id)
    {
        try
        {
            var video = await _repository.GetByIdAsync(id);
            if (video == null)
            {
                return new ApiResponseDto<VideoStreamResponseDto>
                {
                    Success = false,
                    Message = "Video not found"
                };
            }

            if (string.IsNullOrEmpty(video.FileName))
            {
                return new ApiResponseDto<VideoStreamResponseDto>
                {
                    Success = false,
                    Message = "Video file not found"
                };
            }

            var stream = await _blobStorageService.GetVideoStreamAsync(video.FileName);
            
            var response = new VideoStreamResponseDto
            {
                VideoStream = stream,
                ContentType = video.ContentType ?? "application/octet-stream",
                ContentLength = video.FileSize,
                FileName = video.FileName
            };

            return new ApiResponseDto<VideoStreamResponseDto>
            {
                Success = true,
                Message = "Video stream retrieved successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming video with ID {VideoId}", id);
            return new ApiResponseDto<VideoStreamResponseDto>
            {
                Success = false,
                Message = "Failed to stream video",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteVideoAsync(int id)
    {
        try
        {
            var video = await _repository.GetByIdAsync(id);
            if (video == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Video not found"
                };
            }

            // Delete from blob storage
            if (!string.IsNullOrEmpty(video.FileName))
            {
                await _blobStorageService.DeleteVideoAsync(video.FileName);
            }

            // Soft delete from database
            var deleted = await _repository.DeleteAsync(id);
            
            if (deleted)
            {
                _logger.LogInformation("Video with ID {VideoId} deleted successfully", id);
                return new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = "Video deleted successfully",
                    Data = true
                };
            }

            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "Failed to delete video"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting video with ID {VideoId}", id);
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "Failed to delete video",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponseDto<VideoResponseDto>> UpdateVideoAsync(int id, VideoUploadRequestDto request)
    {
        try
        {
            var existingVideo = await _repository.GetByIdAsync(id);
            if (existingVideo == null)
            {
                return new ApiResponseDto<VideoResponseDto>
                {
                    Success = false,
                    Message = "Video not found"
                };
            }

            // Update basic info
            existingVideo.Title = request.Title;
            existingVideo.Description = request.Description;
            existingVideo.UpdatedAt = DateTime.UtcNow;

            var updatedVideo = await _repository.UpdateAsync(existingVideo);
            var responseDto = _mapper.Map<VideoResponseDto>(updatedVideo);

            return new ApiResponseDto<VideoResponseDto>
            {
                Success = true,
                Message = "Video updated successfully",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating video with ID {VideoId}", id);
            return new ApiResponseDto<VideoResponseDto>
            {
                Success = false,
                Message = "Failed to update video",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private bool IsValidVideoFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv" };
        var allowedMimeTypes = new[] { "video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv" };
        
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return allowedExtensions.Contains(extension) && 
               allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
    }

    private string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileName = Path.GetFileNameWithoutExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8];
        
        return $"{fileName}_{timestamp}_{guid}{extension}";
    }
}
