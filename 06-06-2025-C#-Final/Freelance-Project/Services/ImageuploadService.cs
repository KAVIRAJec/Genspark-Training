using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
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

    // Azure Blob Storage implementation

    // private readonly BlobServiceClient _blobServiceClient;
    // private readonly string _containerName;

    // public ImageUploadService(IConfiguration configuration)
    // {
    //     var connectionString = configuration["Azure:StorageConnectionString"];
    //     if (string.IsNullOrEmpty(connectionString))
    //         throw new AppException("Azure Storage connection string is not configured.", 500);

    //     _blobServiceClient = new BlobServiceClient(connectionString);
    //     _containerName = "images";

    //     CreateContainerIfNotExists().GetAwaiter().GetResult();
    // }

    // private async Task CreateContainerIfNotExists()
    // {
    //     try
    //     {
    //         var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    //         await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new AppException($"Failed to create Azure Blob Storage container: {ex.Message}", 500);
    //     }
    // }

    // public virtual async Task<string> UploadImage(IFormFile image)
    // {
    //     if (image == null || image.Length == 0)
    //         throw new AppException("Image file is required.", 400);

    //     try
    //     {
    //         string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
    //         var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    //         var blobClient = containerClient.GetBlobClient(uniqueFileName);

    //         // set content type
    //         var blobHttpHeaders = new BlobHttpHeaders
    //         {
    //             ContentType = image.ContentType
    //         };

    //         // Upload the image
    //         await using var stream = image.OpenReadStream();
    //         await blobClient.UploadAsync(stream, new BlobUploadOptions{ HttpHeaders = blobHttpHeaders });
    //         return blobClient.Uri.ToString();
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new AppException($"Failed to upload image: {ex.Message}", 500);
    //     }
    // }

    // public virtual async Task<bool> DeleteImage(string imageUrl)
    // {
    //     if (string.IsNullOrEmpty(imageUrl))
    //         throw new AppException("Image URL is required.", 400);

    //     try
    //     {
    //         var uri = new Uri(imageUrl);
    //         string blobName = Path.GetFileName(uri.LocalPath);

    //         var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    //         var blobClient = containerClient.GetBlobClient(blobName);

    //         var response = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    //         return response.Value;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new AppException($"Failed to delete image: {ex.Message}", 500);
    //     }

    // }
}