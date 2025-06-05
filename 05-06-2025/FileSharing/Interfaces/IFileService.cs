using FileApp.Models;

namespace FileApp.Interfaces;

public interface IFileService
{
    public Task<FileModel> UploadFile(IFormFile file, string userName);
    public Task<FileModel> GetFile(int id);
    public Task<IEnumerable<FileModel>> GetAllFiles();
    public Task<FileModel> UpdateFile(IFormFile file, string userName);
    public Task<FileModel> DeleteFile(int id);
}