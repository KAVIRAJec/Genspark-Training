using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DBTest.Interfaces
{
    public interface IService<TEntity, TKey> where TEntity : class
    {
        // Create
        TEntity Create(TEntity entity);

        // Read
        IEnumerable<TEntity> GetAll();
        TEntity? GetById(TKey id);

        // Update
        TEntity Update(TEntity entity);

        // Delete
        bool Delete(TKey id);
    }
}
