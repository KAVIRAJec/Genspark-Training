using FileApp.Services;
using FileApp.Misc;
using FileApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FileApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FileOperationController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public FileOperationController(IFileService fileService,
                                    IHubContext<NotificationHub> hubContext)
    {
        _fileService = fileService;
        _hubContext = hubContext;   
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadedFile = await _fileService.UploadFile(file, User.Identity.Name);
            var message = $"{User.Identity.Name} uploaded a new file: {uploadedFile.FileName} ({uploadedFile.Size})";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            return Ok(uploadedFile);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error uploading file: {ex.Message}");

        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(int id)
    {
        try
        {
            var file = await _fileService.GetFile(id);
            if (file == null) return NotFound();
            var contentType = file.FileType switch
            {
                "pdf" => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "txt" => "text/plain",
                _ => "application/octet-stream"
            };

            return File(file.FileContent, contentType, file.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving file: {ex.Message}");
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _fileService.GetAllFiles();
        return Ok(files);
    }

    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateFile(string username, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var updatedFile = await _fileService.UpdateFile(file, username);
        return Ok(updatedFile);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var deletedFile = await _fileService.DeleteFile(id);
        return Ok(deletedFile);
    }
}