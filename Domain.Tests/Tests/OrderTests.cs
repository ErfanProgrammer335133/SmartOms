using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class OrderTests
    {
        private List<OrderItem> items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid() , "title 1" , new ValueObjects.Money(100) , 2 , 3),
            new OrderItem(Guid.NewGuid() , "title 2" , new ValueObjects.Money(50) , 4 , 4),
            new OrderItem(Guid.NewGuid() , "title 3" , new ValueObjects.Money(200) , 1 , 6),
            new OrderItem(Guid.NewGuid() , "title 4" , new ValueObjects.Money(350) , 2 , 2),
        };
        public Order CreateOrderWithStatus(OrderStatusEnum status)
        {
            Order order = new Order(Guid.NewGuid(), items);

            switch (status)
            {
                case OrderStatusEnum.Paid:
                    {
                        order.MarkAsPaid();
                        break;
                    }
                case OrderStatusEnum.InProgress:
                    {
                        order.MarkAsPaid();
                        order.ToInprogress();
                        break;
                    }
                case OrderStatusEnum.Completed:
                    {
                        order.MarkAsPaid();
                        order.ToInprogress();
                        order.Complete();
                        break;
                    }
                case OrderStatusEnum.Canceled:
                    {
                        order.Cancel();
                        break;
                    }
            }
            return order;
        }
        public class OrderItemsTestClass : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new List<OrderItem>() };
                yield return new object[] { null! };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Theory]
        [ClassData(typeof(OrderItemsTestClass))]

        public void Constructor_Should_Throw_DomainValidationException_When_Items_Is_Null_Or_Items_Count_Is0(List<OrderItem> items)
        {
            Assert.Throws<DomainValidationException>(() =>
            new Order(Guid.NewGuid(), items));
        }

        [Fact]
        public void Constructor_should_Create_order_Correctly_When_All_Parameters_Are_Valid()
        {
            Guid customerId = Guid.NewGuid();
            Order order = new Order(customerId, items);

            Assert.NotNull(order);
            Assert.Equal(customerId, order.CustomerId);

            foreach (OrderItem item in items)
            {
                OrderItem? find = order.Items.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(find);
                Assert.Equal(item.ServiceId, find.ServiceId);
                Assert.Equal(item.Title, find.Title);
                Assert.Equal(item.UnitPrice.Amount, find.UnitPrice.Amount);
                Assert.Equal(item.UnitPrice.Currency, find.UnitPrice.Currency);
                Assert.Equal(item.Quantity, find.Quantity);
                Assert.Equal(item.MaxQuantity, find.MaxQuantity);
                Assert.Equal(OrderStatusEnum.Pending, order.Status);
            }
        }

        [Fact]
        public void TotalPrice_Should_Calculate_Items_Price_Correctly()
        {
            Order order = new Order(Guid.NewGuid(), items);

            decimal expected_TotalPrice = 200 + 200 + 200 + 700;
            Assert.Equal(expected_TotalPrice, order.TotalPrice.Amount);
            Assert.Equal(items.First().UnitPrice.Currency, order.TotalPrice.Currency);
        }

        [Fact]
        public void AddItem_Should_Throw_DomainValidationException_When_Item_Is_Null()
        {
            Order order = new Order(Guid.NewGuid(), items);
            Assert.Throws<DomainValidationException>(() => order.AddItem(null!));
        }

        [Theory]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void AddItem_Should_Throw_OrderStatusException_When_Orders_Status_Is_Not_In_Pending(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            OrderItem item = new OrderItem(Guid.NewGuid(), "test", new ValueObjects.Money(100), 2, 3);
            Assert.Throws<OrderStatusException>(() => order.AddItem(item));
        }

        [Fact]
        public void AddItem_Should_Increase_Items_Qunatity_If_ServiceId_And_Price_Of_New_Item_Are_Exist_In_Items()
        {
            Guid serviceId = Guid.NewGuid();

            List<OrderItem> items = new List<OrderItem>
            {
                new OrderItem(serviceId, "title 1", new ValueObjects.Money(100), 2, 3),
                new OrderItem(Guid.NewGuid(), "title 2", new ValueObjects.Money(200), 1, 3)
            };
            OrderItem newItem = new  OrderItem(serviceId, "title 1", new ValueObjects.Money(100), 1, 3);
            Order order = new Order(Guid.NewGuid(), items);
            order.AddItem(newItem);
            int expected_Quantity = 2 + 1; // 3

            OrderItem? find = order.Items.FirstOrDefault(x => x.ServiceId == newItem.ServiceId);
            Assert.NotNull(find);
            Assert.Equal(expected_Quantity, find.Quantity);
        }

        [Fact]
        public void AddItem_Should_Add_New_Item_Correctly_When_Item_Is_Valid_And_Non_Duplaicate()
        {
            Order order = new Order(Guid.NewGuid(), items);

            Guid serviceId = Guid.NewGuid();
            OrderItem newItem = new OrderItem(serviceId, "title 4", new ValueObjects.Money(12), 2, 5);
            order.AddItem(newItem);

            OrderItem? find = order.Items.FirstOrDefault(x => x.ServiceId == serviceId);

            Assert.NotNull(find);
            Assert.Equal(find.Title, newItem.Title);
            Assert.Equal(find.UnitPrice.Amount, newItem.UnitPrice.Amount);
            Assert.Equal(find.UnitPrice.Currency, newItem.UnitPrice.Currency);
            Assert.Equal(find.Quantity, newItem.Quantity);
            Assert.Equal(find.MaxQuantity, newItem.MaxQuantity);

        }

        [Fact]
        public void RemoveItem_Should_Throw_DomainValidationExceptio_When_Item_Not_Found()
        {
            Order order = new Order(Guid.NewGuid(), items);
            Assert.Throws<DomainValidationException>(() => order.RemoveItem(Guid.NewGuid()));
        }

        [Theory]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void RemoveItem_Should_Throw_OrderStatusException_When_Orders_Status_Is_Not_In_Pending(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.Throws<OrderStatusException>(() => order.RemoveItem(Guid.NewGuid()));
        }

        [Fact]
        public void RemoveItem_Should_Remove_Item_Correctly_When_Parameter_Is_Valid_And_Status_Is_In_Pending()
        {
            OrderItem item = new OrderItem(Guid.NewGuid(), "test", new ValueObjects.Money(143), 2, 8);
            List<OrderItem> items = new List<OrderItem>();
            items.Add(item);

            Order order = new Order(Guid.NewGuid(), items);

            int Items_Count_Before_Remove = order.Items.Count ; // = 1
            order.RemoveItem(item.Id);

            OrderItem? find = order.Items.FirstOrDefault(x => x.Id == item.Id);
            Assert.Null(find);
            Assert.Equal(Items_Count_Before_Remove - 1, order.Items.Count);
        }

        [Theory]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void MarkAsPaid_Should_Throw_OrderStatusException_When_OrderStatus_Is_Not_In_Pending(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);

            Assert.Throws<OrderStatusException>(() => order.MarkAsPaid());
        }

        [Fact]
        public void MarkAsPaid_Should_Change_Status_To_Paid_When_Current_Status_Is_Pending()
        {
            Order order = new Order(Guid.NewGuid(), items);

            order.MarkAsPaid();
            Assert.Equal(OrderStatusEnum.Paid, order.Status);
        }

        [Theory]
        [InlineData(OrderStatusEnum.Pending)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void ToInprogress_Should_Throw_OrderStatus_Exception_When_Current_Status_Is_Not_In_Paid(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.Throws<OrderStatusException>(() => order.ToInprogress());
        }

        [Fact]
        public void ToInprogress_Should_Change_Status_To_Inprogress_When_Current_Status_Is_Paid()
        {
            Order order = CreateOrderWithStatus(OrderStatusEnum.Paid);
            order.ToInprogress();
            Assert.Equal(OrderStatusEnum.InProgress, order.Status);
        }

        [Theory]
        [InlineData(OrderStatusEnum.Pending)]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void Complete_Should_Throw_OrderStatus_Exception_When_Current_Status_Is_Not_In_Inprogress(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.Throws<OrderStatusException>(() => order.Complete());
        }

        [Fact]
        public void Complete_Should_Change_Status_To_Completed_When_Current_Status_Is_Inprogress()
        {
            Order order = CreateOrderWithStatus(OrderStatusEnum.InProgress);
            order.Complete();
            Assert.Equal(OrderStatusEnum.Completed, order.Status);
        }

        [Theory]
        [InlineData(OrderStatusEnum.Completed)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void Cancel_Should_throw_OrderStatusException_When_Current_Status_Is_Completed_Or_Canceled(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.Throws<OrderStatusException>(() => order.Cancel());
        }


        [Theory]
        [InlineData(OrderStatusEnum.Pending)]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        public void Cancel_Should_Change_Status_To_Cancel_When_Current_Status_Is_Not_Completed_Or_Canceled(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            order.Cancel();
            Assert.Equal(OrderStatusEnum.Canceled, order.Status);
        }

        [Fact]
        public void IsCompleted_Should_Return_True_When_Orders_Status_Is_Completed()
        {
            Order order = CreateOrderWithStatus(OrderStatusEnum.Completed);
            Assert.True(order.IsCompleted());
        }

        [Theory]
        [InlineData(OrderStatusEnum.Pending)]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Canceled)]
        public void IsCompleted_Should_Return_False_When_Orders_Status_Is_Not_Completed(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.False(order.IsCompleted());
        }

        [Fact]
        public void IsCanceled_Should_Return_True_When_Orders_Status_Is_Canceled()
        {
            Order order = CreateOrderWithStatus(OrderStatusEnum.Canceled);
            Assert.True(order.IsCanceled());
        }

        [Theory]
        [InlineData(OrderStatusEnum.Pending)]
        [InlineData(OrderStatusEnum.Paid)]
        [InlineData(OrderStatusEnum.InProgress)]
        [InlineData(OrderStatusEnum.Completed)]
        public void IsCanceled_Should_Return_False_When_Orders_Status_Is_Not_Canceled(OrderStatusEnum status)
        {
            Order order = CreateOrderWithStatus(status);
            Assert.False(order.IsCanceled());
        }
    }
}
