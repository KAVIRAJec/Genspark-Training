using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;

namespace Freelance_Project.Services;

public class HashingService : IHashingService
{
    public async Task<HashingModel> HashData(HashingModel data)
    {
        if (data == null || string.IsNullOrEmpty(data.Data))
            throw new AppException("Invalid data for hashing.", 400);

        if (data.HashKey == null)
            data.HashKey = RandomNumberGenerator.GetBytes(32);

        string saltedData = data.Data + Encoding.UTF8.GetString(data.HashKey);

        string hashed = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(saltedData));
        data.HashedData = Encoding.UTF8.GetBytes(hashed);
        return data;
    }
    public async Task<bool> VerifyHash(HashingModel data)
    {
        if (data == null || data.HashedData == null || string.IsNullOrEmpty(data.Data) || data.HashKey == null)
            throw new AppException("Invalid data for hash verification.", 400);

        string saltedData = data.Data + Encoding.UTF8.GetString(data.HashKey);
        string hashedData = Encoding.UTF8.GetString(data.HashedData);

        return await Task.Run(() => BCrypt.Net.BCrypt.Verify(saltedData, hashedData));
    }
}