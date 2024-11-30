using System;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repos;

public class BaseRepository<TEntity> where TEntity : class
{
    private readonly BookStoreContext db;

    public BaseRepository(BookStoreContext db)
    {
        this.db = db;
    }
    public List<TEntity> SelectAll()
    {
        return db.Set<TEntity>().ToList();
    }

    public void Add(TEntity entity)
    {
        db.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        db.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("Can't Delete Null");

        db.Set<TEntity>().Remove(entity);
    }
}
