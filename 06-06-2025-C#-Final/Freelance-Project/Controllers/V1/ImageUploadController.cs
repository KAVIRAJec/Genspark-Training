using Freelance_Project.Interfaces;
using Freelance_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ImageUploadController : BaseApiController
{
    private readonly IImageUploadService _imageUploadService;

    public ImageUploadController(IImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        var result = await _imageUploadService.UploadImage(image);
        return result != null ? Success(result) : BadRequest("Image upload failed");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteImage(string imageUrl)
    {
        var result = await _imageUploadService.DeleteImage(imageUrl);
        return result ? Success("Image deleted successfully") : BadRequest("Image deletion failed");
    }
}