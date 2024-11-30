using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repos;

public class Repository<TEntity> : BaseRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly BookStoreContext db;

    public Repository(BookStoreContext db) : base(db)
    {
        this.db = db;
    }

    public TEntity? SelectById(int id, bool track = true)
    {
        if (!track)
            return db.Set<TEntity>().AsNoTracking().FirstOrDefault(e => e.Id == id);

        return db.Set<TEntity>().Find(id);
    }
    
    public void Delete(int id)
    {
        TEntity? entity = db.Set<TEntity>().Find(id);

        if (entity is not null)
            Delete(entity);
    }
}
