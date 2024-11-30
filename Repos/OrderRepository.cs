using System;
using BookStore.Models;

namespace BookStore.Repos;

public class OrderRepository : Repository<Order>
{
    private readonly BookStoreContext db;

    public OrderRepository(BookStoreContext db) : base(db)
    {
        this.db = db;
    }

    public List<Order> SelectOrdersByCustomerId(string customerId)
    {
        return db.Orders
            .Where(o => o.CustomerId == customerId)
            .ToList();
    }
}
