using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;

namespace Freelance_Project.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly FreelanceDBContext _appContext;
        public Repository(FreelanceDBContext appContext)
        {
            _appContext = appContext;
        }
        public async Task<T> Add(T item)
        {
            _appContext.Add(item);
            await _appContext.SaveChangesAsync(); //generate and execute the DML queries for the objects where state is in ['added','modified','deleted']
            return item;
        }
        public abstract Task<T> Delete(K key);
        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _appContext.Entry(myItem).CurrentValues.SetValues(item);
                await _appContext.SaveChangesAsync();
                return myItem;
            }
            throw new KeyNotFoundException($"Item with key {key} not found.");
        }
    }
}