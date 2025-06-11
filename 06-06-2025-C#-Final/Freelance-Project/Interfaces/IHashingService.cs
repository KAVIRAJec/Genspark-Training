using Freelance_Project.Models;

namespace Freelance_Project.Interfaces;

public interface IHashingService
{
        public Task<HashingModel> HashData(HashingModel data);
        public Task<bool> VerifyHash(HashingModel data);
}