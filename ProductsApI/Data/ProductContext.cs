using Microsoft.EntityFrameworkCore;
using ProductsApI.Models;

namespace ProductsApI.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired(false);


            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Person)
                .WithOne()
                .HasForeignKey<Employee>(e => e.PersonId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
