namespace Freelance_Project.Interfaces;

public interface IRepository<K, T> where T : class
{
    public Task<T> Get(K id);
    public Task<IEnumerable<T>> GetAll();
    public Task<T> Add(T entity);
    public Task<T> Update(K id,T entity);
    public Task<T> Delete(K id);
}