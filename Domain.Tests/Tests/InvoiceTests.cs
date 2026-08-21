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
    public class InvoiceTests
    {
        private List<InvoiceItem> items = new List<InvoiceItem>
            {
                new InvoiceItem(Guid.NewGuid(), 2 , "title 1" , new Money(100) , 10),
                new InvoiceItem(Guid.NewGuid(), 3 , "title 2" , new Money(150) , 0),
                new InvoiceItem(Guid.NewGuid(), 1 , "title 3" , new Money(24) , 50)
            };

        private IReadOnlyCollection<CartItem> cartItems = new List<CartItem>
            {
                new CartItem(Guid.NewGuid() , "title 1" , new Money(50) , 3 ,3 , 20),
                new CartItem(Guid.NewGuid() , "title 2" , new Money(150) , 2 ,5 , 0),
                new CartItem(Guid.NewGuid() , "title 3" , new Money(250) , 6 ,7 , 10),
            };

        public class InvoiceItemsTestClass : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { null };
                yield return new object[] { new List<InvoiceItem>()};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        [Theory]
        [ClassData(typeof(InvoiceItemsTestClass))]
        public void Constructor_Should_Throw_DomainValidationException_When_Items_Is_Null_Or_Itens_Count_Is_Equal_To0(List<InvoiceItem> items)
        {
            Assert.Throws<DomainValidationException>(() => new Invoice(Guid.NewGuid(), items));
        }

        [Fact]
        public void Constructor_Should_Calculate_TotalPrice_Correctly()
        {
            Invoice invoice = new Invoice(Guid.NewGuid(), items);
            decimal expected_TotalPrice = 2 * 100 + 3 * 150 + 24;

            Assert.Equal(expected_TotalPrice, invoice.TotalPrice.Amount);
        }
        
        [Fact]
        public void Constructor_Should_Calculate_FinalPrice_Correctly()
        {
            Invoice invoice = new Invoice(Guid.NewGuid(), items);
            decimal expected_FinalPrice = 2 * 100 - 2 * 100 * 10 / 100 + 3 * 150 + (1 * 24 - 1 * 24 * 50 / 100);

            Assert.Equal(expected_FinalPrice, invoice.FinalPrice.Amount);
        }

        [Fact] 
        public void Constructor_Should_Create_Invoice_Correctly_When_All_Parameters_Are_Valid()
        {
            Guid orderId = Guid.NewGuid();
            decimal expected_FinalPrice = 2 * 100 - 2 * 100 * 10 / 100 + 3 * 150 + (1 * 24 - 1 * 24 * 50 / 100);
            decimal expected_TotalPrice = 2 * 100 + 3 * 150 + 24;
            Invoice invoice = new Invoice(orderId, items);

            Assert.NotNull(invoice);
            Assert.Equal(orderId, invoice.OrderId);
            Assert.Equal(expected_TotalPrice, invoice.TotalPrice.Amount);
            Assert.Equal(expected_FinalPrice, invoice.FinalPrice.Amount);

            foreach(InvoiceItem item in items)
            {
                InvoiceItem? find = invoice.Items.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(find);
                Assert.Equal(item.Quantity, find.Quantity);
                Assert.Equal(item.ServiceTitle, find.ServiceTitle);
                Assert.Equal(item.DiscountPercent, find.DiscountPercent);
                Assert.True(item.UnitPrice.Equals(find.UnitPrice));
                Assert.True(item.TotalPrice.Equals(find.TotalPrice));
                Assert.True(item.FinalPrice.Equals(find.FinalPrice));
            }
        }

        public class CartItemsTestClass : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { null };
                yield return new object[] { new List<CartItem>().AsReadOnly() };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        [Theory]
        [ClassData(typeof(CartItemsTestClass))]
        public void CreateInvoice_Should_Throw_DomainValidationException_When_CartItems_Is_Null_Or_CartItems_Count_Is0(IReadOnlyCollection<CartItem> cartItems)
        {
            Assert.Throws<DomainValidationException>(() => Invoice.CreateInvoice(Guid.NewGuid() , cartItems));
        }

        [Fact]
        public void CreateInvoice_Should_Create_Invoice_Correctly_When_All_Parameters_Are_Valid()
        {
            Guid orderId = Guid.NewGuid();
            Invoice invoice = Invoice.CreateInvoice(orderId, cartItems);

            Assert.NotNull(invoice);
            Assert.Equal(orderId, invoice.OrderId);

            foreach (CartItem item in cartItems)
            {
                InvoiceItem? find = invoice.Items.FirstOrDefault(x => x.ServiceId == item.ServiceId);

                Assert.NotNull(find);
                Assert.Equal(item.Quantity, find.Quantity);
                Assert.Equal(item.Title, find.ServiceTitle);
                Assert.Equal(item.DiscountPercent, find.DiscountPercent);
                Assert.True(item.UnitPrice.Equals(find.UnitPrice));
                Assert.True(item.TotalPrice.Equals(find.TotalPrice));
                Assert.True(item.FinalPrice.Equals(find.FinalPrice));
            }
        }
    }
}
