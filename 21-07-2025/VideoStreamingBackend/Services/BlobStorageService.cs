using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using VideoStreamingPlatform.Interfaces;

namespace VideoStreamingPlatform.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        _containerName = configuration["AzureStorage:ContainerName"] ?? "training-videos";
        _logger = logger;
        
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    }

    public async Task<string> UploadVideoAsync(IFormFile file, string fileName)
    {
        try
        {
            // Ensure container exists with private access (no public access)
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            
            var blobClient = _containerClient.GetBlobClient(fileName);
            
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            
            var metadata = new Dictionary<string, string>
            {
                { "OriginalFileName", file.FileName },
                { "UploadDate", DateTime.UtcNow.ToString("O") }
            };
            
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Metadata = metadata
            });
            
            _logger.LogInformation("Successfully uploaded video {FileName} to blob storage", fileName);
            
            // Return the SAS URL instead of direct blob URL
            return GenerateSasUrl(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload video {FileName} to blob storage", fileName);
            throw new InvalidOperationException($"Failed to upload video: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteVideoAsync(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DeleteIfExistsAsync();
            
            if (response.Value)
            {
                _logger.LogInformation("Successfully deleted video {FileName} from blob storage", fileName);
            }
            
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete video {FileName} from blob storage", fileName);
            return false;
        }
    }

    public async Task<Stream> GetVideoStreamAsync(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadStreamingAsync();
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get video stream for {FileName}", fileName);
            throw new InvalidOperationException($"Failed to get video stream: {ex.Message}", ex);
        }
    }

    public async Task<bool> VideoExistsAsync(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.ExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if video {FileName} exists", fileName);
            return false;
        }
    }

    public string GetVideoUrl(string fileName)
    {
        // Return SAS URL for secure access
        return GenerateSasUrl(fileName);
    }

    public async Task<long> GetVideoSizeAsync(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var properties = await blobClient.GetPropertiesAsync();
            return properties.Value.ContentLength;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get video size for {FileName}", fileName);
            return 0;
        }
    }

    private string GenerateSasUrl(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            
            // Check if we can generate SAS tokens (requires account key, not just connection string)
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = fileName,
                    Resource = "b", // blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(24) // Valid for 24 hours
                };
                
                // Set permissions for SAS token
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                
                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
            else
            {
                _logger.LogWarning("Cannot generate SAS URI for blob {FileName}. Using direct blob URL.", fileName);
                return blobClient.Uri.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate SAS URL for {FileName}", fileName);
            return _containerClient.GetBlobClient(fileName).Uri.ToString();
        }
    }
}
