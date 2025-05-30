using System.Collections.Generic;
using System.Threading.Tasks;

namespace FAQChatBot.Interfaces
{

    public interface IRepository<K, T>
    {
        public Task<T> Create(T entity);
        public Task<T> Get(K id);
        public Task<IEnumerable<T>> GetAll();
        public Task<T> Update(K id, T entity);
        public Task<T> Delete(K id);
    }
}