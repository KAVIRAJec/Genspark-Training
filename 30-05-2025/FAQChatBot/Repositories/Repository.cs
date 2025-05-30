using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using FAQChatBot.Interfaces;
using FAQChatBot.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FAQChatBot.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly FAQContext _context;

        public Repository(FAQContext context)
        {
            _context = context;
        }
        public async Task<T> Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> Delete(K id)
        {
            var entity = await Get(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            throw new KeyNotFoundException($"Entity with key {id} not found.");
        }

        public abstract Task<T> Get(K id);
        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K id, T entity)
        {
            var existingEntity = await Get(id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingEntity;
            }
            throw new KeyNotFoundException($"Entity with key {id} not found.");
        }
    }
}