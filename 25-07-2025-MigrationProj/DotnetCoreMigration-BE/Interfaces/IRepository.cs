using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Interfaces;

public interface IRepository<T, K> where T: class
{
    public Task<IEnumerable<T>> GetAll();
    Task<PaginatedResponse<T>> GetAllPaginated(PaginationRequest request);
    Task<T?> GetById(K id);
    Task<T> Create(T item);
    Task<T> Update(T item);
    Task<bool> Delete(K id);
}
