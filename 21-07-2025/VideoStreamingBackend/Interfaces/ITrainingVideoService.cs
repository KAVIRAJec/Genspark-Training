using VideoStreamingPlatform.DTOs;

namespace VideoStreamingPlatform.Interfaces;

public interface ITrainingVideoService
{
    Task<ApiResponseDto<VideoResponseDto>> UploadVideoAsync(VideoUploadRequestDto request);
    Task<ApiResponseDto<VideoListResponseDto>> GetVideosAsync(PaginationRequestDto pagination);
    Task<ApiResponseDto<VideoResponseDto>> GetVideoByIdAsync(int id);
    Task<ApiResponseDto<VideoStreamResponseDto>> GetVideoStreamAsync(int id);
    Task<ApiResponseDto<bool>> DeleteVideoAsync(int id);
    Task<ApiResponseDto<VideoResponseDto>> UpdateVideoAsync(int id, VideoUploadRequestDto request);
}
