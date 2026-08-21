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
    public class CartTests
    {
        public class CartItems : CartTests
        {
            public List<CartItem> items { get; set; } = new List<CartItem>()
            {
                new CartItem (Guid.NewGuid() , "title1" , new Money(100) , 1 , 3 , 0),
                new CartItem (Guid.NewGuid() , "title2" , new Money(120) , 1 , 3 , 10),
                new CartItem (Guid.NewGuid() , "title3" , new Money(500) , 1 , 3 , 0),
                new CartItem (Guid.NewGuid() , "title4" , new Money(250) , 1 , 3 , 15),
            };

            public void AddItemsToCart(Cart cart)
            {

                foreach (CartItem item in items)
                    cart.AddItem(item.ServiceId, item.Title, item.UnitPrice,
                        item.Quantity, item.MaxQuantity, item.DiscountPercent);
            }
        }


        [Fact]
        public void Constructor_Should_Create_Cart_When_Parameters_Are_Correctly()
        {
            Guid customerId = Guid.NewGuid();
            Cart cart = new Cart(customerId);

            Assert.NotNull(cart);
            Assert.Equal(customerId, cart.CustomerId);
            Assert.False(cart.IsCheckout);
        }

        [Fact]
        public void AddItem_Should_Throw_DomainValidationException_When_Title_Is_Empty()
        {
            Guid hypotheticalCustomerId = Guid.NewGuid();
            Cart cart = new Cart(hypotheticalCustomerId);

            Assert.Throws<DomainValidationException>(() =>
                cart.AddItem(
                   Guid.NewGuid() ,
                   "" ,
                   new Money(100) ,
                   2 , 
                   3 , 
                   0
                   )
            );
        }
        
        [Fact]
        public void AddItem_Should_Throw_DomainValidationException_When_Price_Is_Null()
        {
            Guid hypotheticalCustomerId = Guid.NewGuid();
            Cart cart = new Cart(hypotheticalCustomerId);

            Assert.Throws<DomainValidationException>(() =>
                cart.AddItem(
                   Guid.NewGuid() ,
                   "test title" ,
                   null ,
                   2 , 
                   3 , 
                   0
                   )
            );
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void AddItem_Should_Throw_DomainValidationException_When_Percent_Is_Greater_Than100_Or_Less_Than0(decimal percent)
        {
            Guid hypotheticalCustomerId = Guid.NewGuid();
            Cart cart = new Cart(hypotheticalCustomerId);

            Assert.Throws<DomainValidationException>(() =>
                cart.AddItem(
                   Guid.NewGuid() ,
                   "test title" ,
                   new Money(100) ,
                   2 , 
                   3 , 
                   percent
                   )
            );
        }

        [Fact]
        public void AddItem_should_Increase_Quantity_When_ServiceId_And_Price_Of_New_Item_Are_Exist()
        {
            Guid serviceId = Guid.NewGuid();
            Cart cart = new Cart(Guid.NewGuid());

            cart.AddItem(serviceId, "test title", new Money(100), 1, 3, 0); // Now quantity is 1;
            cart.AddItem(serviceId, "test title", new Money(100), 1, 3, 0);

            int expectedQuantity = 2;
            int realQuantity = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId).Quantity;
            Assert.Equal(expectedQuantity, realQuantity);
        }
        
        [Fact]
        public void AddItem_should_Calculate_Discount_correctly()
        {
            Guid serviceId = Guid.NewGuid();
            Cart cart = new Cart(Guid.NewGuid());

            cart.AddItem(serviceId, "test title", new Money(100), 2, 3, 10);

            decimal excpectedPrice = 2 * 100 - 10 * 200 / 100;
            decimal realPrice = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId).FinalPrice.Amount;

            Assert.Equal(excpectedPrice, realPrice);
        }

        [Fact]
        public void AddItem_Should_Create_CartItem_Correctly_When_All_The_Parameters_Are_Correct()
        {
            Guid serviceId = Guid.NewGuid();
            Cart cart = new Cart(Guid.NewGuid());

            int prevItemsCount = cart.Items.Count;
            cart.AddItem(serviceId, "test title", new Money(100), 2, 3, 0);

            string expectedTitle = "test title";
            decimal expectedPrice = 100 * 2;
            int expectedQuantity = 2;
            int expectedMaxQuantity = 3;
  
            CartItem? item = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId);

            Assert.NotNull(item);
            Assert.Equal(cart.Items.Count  ,prevItemsCount + 1);
            Assert.Equal(expectedTitle ,item.Title);
            Assert.Equal(expectedPrice ,item.FinalPrice.Amount);
            Assert.Equal(expectedQuantity, item.Quantity);
            Assert.Equal(expectedMaxQuantity, item.MaxQuantity);
        }

        [Fact]
        public void RemoveItem_Should_Throw_DomainValidationException_whent_itemId_Was_Not_Founded()
        {
            Cart cart = new Cart(Guid.NewGuid());

            Guid FalseItemId = Guid.NewGuid();
            Assert.Throws<DomainValidationException>(() => cart.RemoveItem(FalseItemId));
        }

        [Fact]
        public void RemoveItem_Should_Remove_CartItem_When_ItemId_Existed()
        {
            Cart cart = new Cart(Guid.NewGuid());

            Guid serviceId = Guid.NewGuid();
            cart.AddItem(serviceId, "tet title", new Money(100), 2, 3, 0);
            int Count_Before_Remove = cart.Items.Count; // = 1;
            CartItem? item = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId);

            cart.RemoveItem(item.Id);
            Assert.Equal(Count_Before_Remove - 1, cart.Items.Count); // = 0;
        }

        [Fact]
        public void Checkout_Should_Throw_DomainValidationException_When_Count_Is0()
        {
            Cart cart = new Cart(Guid.NewGuid());
            Assert.Throws<DomainValidationException>(() => cart.CheckOut());
        }
        
        [Fact]
        public void Checkout_Should_Throw_DomainValidationException_When_IsCheckout_Is_True()
        {
            Cart cart = new Cart(Guid.NewGuid());
            Guid serviceId = Guid.NewGuid();
            cart.AddItem(serviceId, "tet title", new Money(100), 2, 3, 0);
            cart.CheckOut(); // IsCheckout turn to true

            Assert.Throws<DomainValidationException>(() => cart.CheckOut());
        }

        [Fact]
        public void Checkout_Should_Convert_CartItems_To_Order_Correctly()
        {
            Cart cart = new Cart(Guid.NewGuid());
            CartItems items = new();
            items.AddItemsToCart(cart);
            
            Order order = cart.CheckOut();

            Assert.NotNull(order);
            Assert.Equal(cart.FinalPrice.Amount, order.TotalPrice.Amount);

            foreach(CartItem item in items.items)
            {
                OrderItem? orderItem = order.Items.FirstOrDefault(x => x.ServiceId == item.ServiceId);
                Assert.NotNull(orderItem);
                Assert.Equal(item.FinalPrice.Amount, orderItem.TotalPrice.Amount);
                Assert.Equal(item.Quantity, orderItem.Quantity);
                Assert.Equal(item.MaxQuantity, orderItem.MaxQuantity);
            }
        }
        [Fact]
        public void IsCheckout_Should_True_After_Checkout()
        {
            Cart cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), "t1", new Money(100), 2, 3, 10);

            cart.CheckOut();
            Assert.True(cart.IsCheckout);
        }

        [Fact]
        public void Clear_Should_Clear_Items_Correctly()
        {
            Cart cart = new Cart(Guid.NewGuid());
            CartItems items = new();
            items.AddItemsToCart(cart);

            int expectedCount = 0;
            cart.Clear();

            Assert.Equal(expectedCount, cart.Items.Count);
        }
        
        [Fact]
        public void Clear_Should_False_IsCheckout()
        {
            Cart cart = new Cart(Guid.NewGuid());
            CartItems items = new();
            items.AddItemsToCart(cart);
            cart.Clear();

            Assert.False(cart.IsCheckout);
        }

        [Fact]
        public void TotalPrice_Should_Calculate_Sum_Of_All_Items_Prices_Correctly()
        {
            Cart cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), "t1", new Money(100), 2, 3, 10);
            cart.AddItem(Guid.NewGuid(), "t2", new Money(50), 3, 3, 5);

            decimal expectedPrice = 2 * 100 + 50 * 3;
            Assert.Equal(expectedPrice, cart.TotalPrice.Amount);
        }
        
        [Fact]
        public void FinalPrice_Should_Calculate_Sum_Of_All_Items_Prices_With_Discount_Correctly()
        {
            Cart cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), "t1", new Money(100), 2, 3, 10);
            cart.AddItem(Guid.NewGuid(), "t2", new Money(50), 3, 3, 0);

            decimal expectedPrice = 2 * 100 - 2 * 100 * 10 / 100 + 3 * 50;

            Assert.Equal(expectedPrice, cart.FinalPrice.Amount);
        }

        [Fact]
        public void HasItem_Should_Return_True_When_Cart_Has_Item()
        {
            Cart cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), "t1", new Money(100), 2, 3, 10);

            Assert.True(cart.HasItems());
        }
        
        [Fact]
        public void HasItem_Should_Return_False_When_Cart_Has_No_Item()
        {
            Cart cart = new Cart(Guid.NewGuid());
            Assert.False(cart.HasItems());
        }
    }
}
