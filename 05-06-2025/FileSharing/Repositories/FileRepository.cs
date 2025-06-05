using FileApp.Repositories;
using FileApp.Contexts;
using FileApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FileApp.Repositories;
public class FileRepository : Repository<int, FileModel>
{
    public FileRepository(FileAppContext fileAppContext) : base(fileAppContext)
    {
    }

    public override async Task<FileModel> Get(int key)
    {
        var file = await _fileAppContext.Files.Include(f => f.User).FirstOrDefaultAsync(f => f.Id == key);
        return file ?? throw new KeyNotFoundException($"File with ID {key} not found.");
    }

    public override async Task<IEnumerable<FileModel>> GetAll()
    {
        var files = await _fileAppContext.Files.Include(f => f.User).ToListAsync();
        return files ?? new List<FileModel>();
    }
}