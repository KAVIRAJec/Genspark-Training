namespace Freelance_Project.Interfaces;

public interface IImageUploadService
{
    Task<string> UploadImage(IFormFile image);
    Task<bool> DeleteImage(string imageUrl);
}