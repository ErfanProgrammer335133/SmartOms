using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class OrderITemTests
    {
        [Fact]
        public void Constructor_Should_Throw_DomainValidationException_When_Tiltle_Is_Empty()
        {
            Assert.Throws<DomainValidationException>(() =>
            new OrderItem(Guid.NewGuid(), "", new Money(100), 2, 2));
        }

        [Fact]
        public void Constructor_Should_Throw_MoneyValidationException_When_UnitPrice_Is_Null()
        {
            Assert.Throws<MoneyValidationException>(() =>
            new OrderItem(Guid.NewGuid(), "test title", null!, 2, 2));

        }

        [Theory]
        [InlineData(0 , 1)]
        [InlineData(2 , 1)]
        public void Constructor_Should_Throw_DomainValidationException_When_Quantity_Is_Less_Than_Or_Equal_To0_Or_Greater_Than_MaxQuantity(int quantity , int maxQuantity)
        {
            Assert.Throws<DomainValidationException>(() =>
            new OrderItem(Guid.NewGuid(), "test title", new Money(100), quantity, maxQuantity));

        }

        [Fact]
        public void Constructor_Should_Create_OrderItem_Correctly_When_All_Parameters_Are_Valid()
        {
            Guid serviceId = Guid.NewGuid();
            string title = "Test Title";
            Money price = new Money(100);
            int quantity = 3;
            int maxQuantity = 4;

            OrderItem item = new OrderItem(serviceId , title , price , quantity , maxQuantity);

            Assert.NotNull(item);
            Assert.Equal(serviceId, item.ServiceId);
            Assert.Equal(title, item.Title);
            Assert.Equal(price.Amount, item.UnitPrice.Amount);
            Assert.Equal(price.Currency, item.UnitPrice.Currency);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(maxQuantity, item.MaxQuantity);
            Assert.Equal(DateTime.UtcNow.Year, item.CreatedAt.Year);
            Assert.Equal(DateTime.UtcNow.Month, item.CreatedAt.Month);
            Assert.Equal(DateTime.UtcNow.Day, item.CreatedAt.Day);
        }

        [Fact]
        public void TotalPrice_Shoild_Calculate_Sum_Of_Prices_Correctly()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 3, 3);
            decimal expected_TotalPrice = 3 * 150;
            Assert.Equal(expected_TotalPrice, item.TotalPrice.Amount);
        }

        [Fact]
        public void IncreaseQuantity_Should_Throw_DomainValidationException_When_Count_Is_Less_Than0()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            Assert.Throws<DomainValidationException>(() => item.IncreaseQuantity(-1));
        }
        
        [Fact]
        public void IncreaseQuantity_Should_Throw_DomainValidationException_When_Sum_Of_Conut_And_Quantity_Is_Greater_Than_MaxQuantity()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            Assert.Throws<DomainValidationException>(() => item.IncreaseQuantity(2));
        }

        [Fact]
        public void IncreaseQuantity_Should_Increase_Qunatity_When_Count_Is_Valid()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            item.IncreaseQuantity(1);
            int expected_Qunatity = 2 + 1;

            Assert.Equal(expected_Qunatity, item.Quantity);
        }

        [Fact]
        public void DecreaseQuantity_Should_Throw_DomainValidationException_When_Count_Is_Less_Than0()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            Assert.Throws<DomainValidationException>(() => item.DecreaseQuantity(-1));
        }

        [Fact]
        public void DecreaseQuantity_Should_Throw_DomainValidationException_When_Quantity_Minus_Count_Is_Equal_Or_Less_Than0()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            Assert.Throws<DomainValidationException>(() => item.DecreaseQuantity(2));
        }

        [Fact]
        public void DecreaseQuantity_Should_Decrease_Qunatity_When_Count_Is_Valid()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "title", new Money(150), 2, 3);
            item.DecreaseQuantity(1);
            int expected_Qunatity = 2 - 1;

            Assert.Equal(expected_Qunatity, item.Quantity);
        }
    }
}
