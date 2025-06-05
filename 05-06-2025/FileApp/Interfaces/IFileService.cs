using FileApp.Models;

namespace FileApp.Interfaces;

public interface IFileService
{
    public Task<FileModel> UploadFile(IFormFile file);
    public Task<FileModel> GetFile(int id);
    public Task<IEnumerable<FileModel>> GetAllFiles();
    public Task<FileModel> UpdateFile(int id, IFormFile file);
    public Task<FileModel> DeleteFile(int id);
}