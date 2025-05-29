using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly BankContext _context;

        public Repository(BankContext context)
        {
            _context = context;
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Add(T item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync(); //generate and execute the DML queries for the objects where state is in ['added','modified','deleted']
            return item;
        }

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _context.Entry(myItem).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return myItem;
            }
            throw new KeyNotFoundException($"Item with key {key} not found.");
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            throw new KeyNotFoundException($"Item with key {key} not found.");
        }
    }
}