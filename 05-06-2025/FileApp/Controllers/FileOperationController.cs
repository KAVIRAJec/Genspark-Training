using FileApp.Services;
using FileApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileApp.Controllers;

public class FileOperationController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileOperationController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadedFile = await _fileService.UploadFile(file);
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
            return Ok(file);
            // return File(fileModel.Content, "application/octet-stream", fileModel.FileName);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var updatedFile = await _fileService.UpdateFile(id, file);
        return Ok(updatedFile);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var deletedFile = await _fileService.DeleteFile(id);
        return Ok(deletedFile);
    }
}