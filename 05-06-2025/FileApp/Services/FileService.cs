using FileApp.Interfaces;
using FileApp.Models;
using System.Threading.Tasks;

namespace FileApp.Services;

public class FileService : IFileService
{
    private readonly IRepository<int, FileModel> _fileRepository;
    public FileService(IRepository<int, FileModel> fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<FileModel> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");
            
            string fileSize;
            if (file.Length >= 1024 * 1024 * 1024)
                fileSize = $"{file.Length / (1024.0 * 1024 * 1024):F2} GB";
            else if (file.Length >= 1024 * 1024)
                fileSize = $"{file.Length / (1024.0 * 1024):F2} MB";
            else if (file.Length >= 1024)
                fileSize = $"{file.Length / 1024.0:F2} KB";
            else
                fileSize = $"{file.Length} bytes";
            string fileType = System.IO.Path.GetExtension(file.FileName)?.TrimStart('.').ToLower() ?? "unknown";
            var fileModel = new FileModel
            {
                FileName = file.FileName,
                Size = fileSize,
                FileType = fileType,
                CreatedDate = DateTime.UtcNow,
            };

            await _fileRepository.Add(fileModel);
            return fileModel;
        } catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<FileModel> GetFile(int id)
    {
        try
        {
        if (id <= 0)
            throw new ArgumentException("Invalid file ID.");
            return await _fileRepository.Get(id);
        } catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<IEnumerable<FileModel>> GetAllFiles()
    {
        try
        {
            return await _fileRepository.GetAll();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<FileModel> UpdateFile(int id, IFormFile file)
    {
        throw new NotImplementedException("UpdateFile method is not implemented yet.");
    }
    public async Task<FileModel> DeleteFile(int id)
    {
        throw new NotImplementedException("DeleteFile method is not implemented yet.");
    }
}