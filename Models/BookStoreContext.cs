using BookStore.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookStore.Models;

public class BookStoreContext : IdentityDbContext
{
    private readonly IConfiguration config;

    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Admin> Admins { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    public BookStoreContext(DbContextOptions<BookStoreContext> options, IConfiguration _config) : base(options)
    {
        config = _config;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OrderDetails>().HasKey(e => new { e.OrderId, e.BookId });


        #region Seed Roles
        List<IdentityRole> roles = new List<IdentityRole>()
        {
            new IdentityRole() { Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole() { Name = "Customer", NormalizedName = "CUSTOMER" },
        };

        builder.Entity<IdentityRole>().HasData(roles);
        #endregion

        #region Seed Admins
        List<AdminConfig>? adminsConfig = config.GetSection("Admins").Get<List<AdminConfig>>();
        PasswordHasher<Admin> hasher = new PasswordHasher<Admin>();
        if (adminsConfig != null && adminsConfig.Any())
        {
            List<Admin> admins = adminsConfig.Select(config => new Admin
            {
                UserName = config.UserName,
                NormalizedUserName = config.UserName.ToUpper(),
                Email = config.Email,
                NormalizedEmail = config.Email.ToUpper(),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new Admin(), config.Password)
            }).ToList();

            builder.Entity<Admin>().HasData(admins);

            List<IdentityUserRole<string>> userRoles = admins.Select(admin => new IdentityUserRole<string>
            {
                UserId = admin.Id,
                RoleId = roles[0].Id
            }).ToList();

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
        #endregion
    }
}
