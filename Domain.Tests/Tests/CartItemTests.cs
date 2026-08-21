using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class CartItemTests
    {
        [Fact]
        public void Should_Throw_DomainValidationException_when_UnitPrice_Is_Null()
        {
            Assert.Throws<MoneyValidationException>(() => 
            new CartItem(Guid.NewGuid(), "test title", null, 1, 2, 0));
        }

        [Theory]
        [InlineData(0 , 3)]
        [InlineData(4 , 3)]
        public void Should_Throw_DomainValidationException_When_Quantity_Is_Less_Than_Or_Equal_To0_Or_Greater_Than_MaxQuantity
            (int quantity , int maxQuantity)
        {
            Assert.Throws<DomainValidationException>(() =>
            new CartItem(Guid.NewGuid(), "test title", new Money(100), quantity , maxQuantity, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Should_Throw_DomainValidationException_When_DiscountPercent_Is_Less_Than0_Or_Greater_Than100(int percent)
        {
            Assert.Throws<DomainValidationException>(() =>
            new CartItem(Guid.NewGuid(), "test title", new Money(100), 2 , 3 , percent));
        }

        [Fact]
        public void Constructor_Should_Create_CartItem_When_All_Parameters_Are_Correct()
        {
            Guid serviceId = Guid.NewGuid();
            string title = "test title";
            Money price = new Money(100);
            int quantity = 2;
            int maxQuantity = 3;
            decimal discountPercent = 10;
            CartItem item = new CartItem(serviceId , title , price , quantity , maxQuantity , discountPercent);

            Assert.NotNull(item);
            Assert.Equal(serviceId, item.ServiceId);
            Assert.Equal(title, item.Title);
            Assert.Equal(price.Amount, item.UnitPrice.Amount);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(maxQuantity, item.MaxQuantity);
            Assert.Equal(discountPercent, item.DiscountPercent);
        }

        [Fact]
        public void TotalPrice_Should_Calculate_Items_Price_Correctly()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            decimal expectedPrice = 2 * 100;
            Assert.Equal(expectedPrice, item.TotalPrice.Amount);
        }
        
        [Fact]
        public void FinalPrice_Should_Calculate_Items_Price_With_Discount_Correctly()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            decimal expectedPrice = 2 * 100 - 2 * 100 * 10 / 100;
            Assert.Equal(expectedPrice, item.FinalPrice.Amount);
        }

        [Fact]
        public void IncreaseQuantity_Should_Throw_DomainValidationException_When_Count_Is_Less_Than0()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            Assert.Throws<DomainValidationException>(() => item.IncreaseQuantity(-1));
        }
        
        [Fact]
        public void IncreaseQuantity_Should_Throw_DomainValidationException_When_Count_Plus_Quantity_Is_Greater_Than_MaxQuantity()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            Assert.Throws<DomainValidationException>(() => item.IncreaseQuantity(2));
        }
        
        [Fact]
        public void IncreaseQuantity_Should_Increase_Items_Quantity_When_Count_IS_Valid()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 5, 10);
            item.IncreaseQuantity(2);
            int expectedCount = 4;
            Assert.Equal(expectedCount, item.Quantity);
        }

        [Fact]
        public void DecreaseQuantity_Should_Throw_DomainValidationException_When_Count_Is_Less_Than0()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            Assert.Throws<DomainValidationException>(() => item.DecreaseQuantity(-1));
        }

        [Fact]
        public void DecreaseQuantity_Should_Throw_DomainValidationException_When_Quanity_Minus_Count_Is_Equal_Or_Less_Than0()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 3, 10);
            Assert.Throws<DomainValidationException>(() => item.DecreaseQuantity(2));
        }

        [Fact]
        public void DecreaseQuantity_Should_Decrease_Items_Quantity_When_Count_IS_Valid()
        {
            CartItem item = new CartItem(Guid.NewGuid(), "test title", new Money(100), 2, 5, 10);
            item.DecreaseQuantity(1);
            int expectedCount = 1;
            Assert.Equal(expectedCount, item.Quantity);
        }

        [Fact]
        public void SetUnitPrice_Should_Throw_MoneyValidationException_When_Price_Is_Null()
        {
            var item = new CartItem(Guid.NewGuid(), "test", new Money(100), 1, 3, 0);
            Assert.Throws<MoneyValidationException>(() => item.SetUnitPrice(null));
        }

        [Fact]
        public void SetUnitPrice_Should_Set_Price_Correctly_When_Price_Is_Valid()
        {
            var item = new CartItem(Guid.NewGuid(), "test", new Money(100), 1, 3, 0);
            var newPrice = new Money(200);
            item.SetUnitPrice(newPrice);
            Assert.Equal(200, item.UnitPrice.Amount);
        }
    }
}
