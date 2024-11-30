using BookStore.Models;
using BookStore.Repos;

namespace BookStore.UnitOfWorks;

public class UnitOfWork
{
    private readonly BookStoreContext db;
    private Repository<Book>? books;
    private Repository<Author>? authors;
    private Repository<Category>? categories;
    private OrderRepository? orders;
    private BaseRepository<OrderDetails>? orderDetails;
    public Repository<Book> Books
    {
        get
        {
            if (books == null)
            {
                books = new Repository<Book>(db);
            }
            return books;
        }
    }

    public Repository<Author> Authors
    {
        get
        {
            if (authors == null)
            {
                authors = new Repository<Author>(db);
            }
            return authors;
        }
    }

    public Repository<Category> Categories
    {
        get
        {
            if (categories == null)
            {
                categories = new Repository<Category>(db);
            }
            return categories;
        }
    }

    public OrderRepository Orders
    {
        get
        {
            if (orders == null)
            {
                orders = new OrderRepository(db);
            }
            return orders;
        }
    }

    public BaseRepository<OrderDetails> OrderDetails
    {
        get
        {
            if (orderDetails == null)
            {
                orderDetails = new BaseRepository<OrderDetails>(db);
            }
            return orderDetails;
        }
    }

    public UnitOfWork(BookStoreContext db)
    {
        this.db = db;
    }

    public void Save()
    {
        db.SaveChanges();
    }
}
