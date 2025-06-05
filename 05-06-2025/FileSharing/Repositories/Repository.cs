using FileApp.Contexts;
using FileApp.Models;
using FileApp.Interfaces;

namespace FileApp.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly FileAppContext _fileAppContext;
        public Repository(FileAppContext fileAppContext)
        {
            _fileAppContext = fileAppContext;
        }
        public async Task<T> Add(T item)
        {
            _fileAppContext.Add(item);
            await _fileAppContext.SaveChangesAsync(); //generate and execute the DML queries for the objects where state is in ['added','modified','deleted']
            return item;
        }
        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _fileAppContext.Remove(item);
                await _fileAppContext.SaveChangesAsync();
                return item;
            }
            throw new KeyNotFoundException($"Item with key {key} not found.");
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _fileAppContext.Entry(myItem).CurrentValues.SetValues(item);
                await _fileAppContext.SaveChangesAsync();
                return myItem;
            }
            throw new KeyNotFoundException($"Item with key {key} not found.");
        }
    }
}