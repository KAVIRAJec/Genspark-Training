namespace BankingApp.Interfaces
{
    public interface IRepository<K, T>
    {
        Task<T> Get(K id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(K id,T entity);
        Task<T> Delete(K id);
    }
}
