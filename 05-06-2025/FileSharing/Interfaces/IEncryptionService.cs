using FileApp.Models;

namespace FileApp.Interfaces
{
    public interface IEncryptionService
    {
         public Task<EncryptModel> EncryptData(EncryptModel data);
    }
}