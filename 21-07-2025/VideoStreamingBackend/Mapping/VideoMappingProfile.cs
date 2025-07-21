using AutoMapper;
using VideoStreamingPlatform.Models;
using VideoStreamingPlatform.DTOs;

namespace VideoStreamingPlatform.Mapping;

public class VideoMappingProfile : Profile
{
    public VideoMappingProfile()
    {
        // TrainingVideo -> VideoResponseDto
        CreateMap<TrainingVideo, VideoResponseDto>();
        
        // VideoUploadRequestDto -> TrainingVideo
        CreateMap<VideoUploadRequestDto, TrainingVideo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BlobUrl, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FileSize, opt => opt.Ignore())
            .ForMember(dest => dest.ContentType, opt => opt.Ignore())
            .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
    }
}
