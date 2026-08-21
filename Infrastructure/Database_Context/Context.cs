using Dmain.Entities;
using Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database_Context
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Wallet> Wallets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RowVersionGuard.AddRowVersionToAll(modelBuilder,

                new List<Type>
                {
                     typeof(BusinessService) ,
                     typeof(Cart) ,
                     typeof(CartItem) ,
                     typeof(Customer) ,
                     typeof(Order) ,
                     typeof(OrderItem) ,
                     typeof(PaymentTransaction) ,
                     typeof(User) ,
                     typeof(Wallet) ,
                }
            );
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.HashPassword).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .HasDatabaseName("IX_User_username&passwod")
                .IsUnique();

            modelBuilder.Entity<BusinessService>().HasKey(u => u.Id);
            modelBuilder.Entity<BusinessService>().Property(u => u.Title).IsRequired();
            modelBuilder.Entity<BusinessService>().Property(u => u.Explanation).IsRequired();
            modelBuilder.Entity<BusinessService>()
                .OwnsMoney(u => u.Price, "Price");

            modelBuilder.Entity<Cart>().HasKey(u => u.Id);
            modelBuilder.Entity<Cart>().Property(u => u.CustomerId).IsRequired();
            modelBuilder.Entity<Cart>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Cart>(u => u.CustomerId);
            modelBuilder.Entity<Cart>()
                .OwnsMany(typeof(CartItem), "_items", item =>
                {
                    item.WithOwner().HasForeignKey("CartId");

                    item.Property<Guid>("Id");
                    item.HasKey("Id");
                 });

            modelBuilder.Entity<Customer>().HasKey(u => u.Id);
            modelBuilder.Entity<Customer>().Property(c => c.FullName).IsRequired();
            modelBuilder.Entity<Customer>().Property(c => c.Phone).IsRequired();
            modelBuilder.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId);

            modelBuilder.Entity<Order>().HasKey(u => u.Id);
            modelBuilder.Entity<Order>()
                .OwnsMany(typeof(OrderItem), "_items", item =>
                {
                    item.WithOwner().HasForeignKey("OrderId");

                    item.Property<Guid>("Id");
                    item.HasKey("Id");
                });
            modelBuilder.Entity<Order>()
                .HasOne<Customer>()      
                .WithMany()
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Wallet>().HasKey(u => u.Id);
            modelBuilder.Entity<Wallet>()
                .OwnsMany(typeof(PaymentTransaction), "_transactions", transaction =>
                {
                    transaction.WithOwner().HasForeignKey("WalletId");

                    transaction.Property<Guid>("Id");
                    transaction.HasKey("Id");
                });
            modelBuilder.Entity<Wallet>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Wallet>(w => w.CustomerId);

        }
    }
}
