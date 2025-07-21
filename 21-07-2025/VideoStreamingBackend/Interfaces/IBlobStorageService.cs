namespace VideoStreamingPlatform.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadVideoAsync(IFormFile file, string fileName);
    Task<bool> DeleteVideoAsync(string fileName);
    Task<Stream> GetVideoStreamAsync(string fileName);
    Task<bool> VideoExistsAsync(string fileName);
    string GetVideoUrl(string fileName);
    Task<long> GetVideoSizeAsync(string fileName);
}
