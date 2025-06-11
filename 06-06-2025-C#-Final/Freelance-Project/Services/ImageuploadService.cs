using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.Extensions.Options;

namespace Freelance_Project.Services;

public class ImageUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;

    public ImageUploadService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }
    public virtual async Task<string> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new AppException("Image file is required.", 400);

        await using var stream = image.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Folder = "Freelance_Project/ProfileURLs",
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString();
    }

    public virtual async Task<bool> DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            throw new AppException("Image URL is required.", 400);

        var uri = new Uri(imageUrl);
        var segments = uri.AbsolutePath.Split('/');
        var versionIndex = Array.FindIndex(segments, s => s.StartsWith("v") && s.Skip(1).All(char.IsDigit));

        var publicIdWithExtension = string.Join("/", segments.Skip(versionIndex + 1));
        var publicId = Path.ChangeExtension(publicIdWithExtension, null);

        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };
        var result = await _cloudinary.DestroyAsync(deleteParams);
        if (result.Result != "ok")
            throw new AppException("Failed to delete image.", 500);
        return true;
    }
}