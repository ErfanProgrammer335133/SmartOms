using Domain.Entities;
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
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentTransaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RowVersionGuard.AddRowVersionToAll(modelBuilder,

                new List<Type>
                {
                     typeof(BusinessService) ,
                     typeof(Cart) ,
                     typeof(Customer) ,
                     typeof(Order) ,
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
            modelBuilder.Entity<User>()
                .OwnsOne(x => x.Phone, mobile =>
                {
                    mobile.Property(m => m.PhoneNumber)
                    .HasColumnName("Mobile");
                });

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
                .Navigation(c => c.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Cart>()
                .HasMany<CartItem>(x => x.Items)
                .WithOne(c => c.Cart)
                .HasForeignKey(c => c.CartId);

            modelBuilder.Entity<CartItem>()
                .OwnsOne(x => x.UnitPrice, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice");

                    money.Property(m => m.Currency)
                    .HasColumnName("Currency");
                });


            modelBuilder.Entity<Customer>().HasKey(u => u.Id);
            modelBuilder.Entity<Customer>().Property(c => c.FullName).IsRequired();
            modelBuilder.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId);

            modelBuilder.Entity<Customer>()
                .OwnsOne(x => x.Email, email =>
                {
                    email.Property(m => m.Address)
                    .HasColumnName("EmailAddess");
                });

            modelBuilder.Entity<Order>().HasKey(u => u.Id);
            modelBuilder.Entity<Order>()
                .HasMany<OrderItem>(x => x.Items)
                .WithOne(c => c.Order)
                .HasForeignKey(c => c.OrderId);


            modelBuilder.Entity<OrderItem>()
                .OwnsOne(x => x.UnitPrice, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice");

                    money.Property(m => m.Currency)
                    .HasColumnName("Currency");
                });
            modelBuilder.Entity<OrderItem>()
                .Ignore(x => x.TotalPrice);

            modelBuilder.Entity<Wallet>().HasKey(u => u.Id);
            modelBuilder.Entity<Wallet>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId);

            modelBuilder.Entity<Wallet>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Wallet>(w => w.CustomerId);

            modelBuilder.Entity<Wallet>()
                .OwnsOne(x => x.Balance, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("Balence");
                });

            modelBuilder.Entity<PaymentTransaction>()
                .OwnsOne(x => x.Amount, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("Amount");

                    money.Property(m => m.Currency)
                    .HasColumnName("Currency");
                });
            modelBuilder.Entity<PaymentTransaction>()
                .OwnsOne(x => x.CurrentBalance, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("CurrentBalence");
                });
            modelBuilder.Entity<PaymentTransaction>()
                .OwnsOne(x => x.PreviousBalance, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("PreviousBalence");
                });

        }
    }
}
