using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DBTest.Interfaces
{
    public interface IRepository<K, T> where K : class
    {
        // Create
        K Add(K entity);

        // Read
        IEnumerable<K> GetAll();
        K? GetById(T id);
        IEnumerable<K> Find(Expression<Func<K, bool>> predicate);

        // Update
        K Update(K entity);

        // Delete
        bool Delete(T id);
    }
}
