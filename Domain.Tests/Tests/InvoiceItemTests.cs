using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    public class InvoiceItemTests
    {
        [Fact]
        public void Constructor_Should_Throw_DomainValidationException_When_Quantity_Is_Equal_Or_Lesss_Than0()
        {
            Assert.Throws<DomainValidationException>(() =>
            new InvoiceItem(Guid.NewGuid(), 0, "title", new ValueObjects.Money(100), 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Constructor_Should_Throw_DomainValidationException_When_DiscountPercent_Is_Lesss_Than0_Or_Greater100(decimal percent)
        {
            Assert.Throws<DomainValidationException>(() =>
            new InvoiceItem(Guid.NewGuid(), 1, "title", new ValueObjects.Money(100), percent));
        }

        [Fact]
        public void Constructor_Should_Throw_DomainValidationException_When_ServiceTitle_Is_Empty()
        {
            Assert.Throws<DomainValidationException>(() =>
            new InvoiceItem(Guid.NewGuid(), 2, "", new ValueObjects.Money(100), 0));
        }

        [Fact]
        public void Constructoe_Should_Calculate_TotalPrice_And_FinalPrice_Correctly()
        {
            InvoiceItem item = new InvoiceItem(Guid.NewGuid(), 6, "title 3", new ValueObjects.Money(412), 50);
            decimal expected_TotalPrice = 6 * 412;
            decimal expected_FinalPrice = 6 * 412 - (6 * 412 * 50) / 100;

            Assert.Equal(expected_TotalPrice, item.TotalPrice.Amount);
            Assert.Equal(expected_FinalPrice, item.FinalPrice.Amount);
        }

        [Fact]
        public void Constructoe_Should_Create_InvoiceItem_When_All_Parameters_Are_Valid()
        {
            Guid serviceId = Guid.NewGuid();
            string serviceTitle = "title 3";
            int quanity = 6;
            Money unitPrice = new ValueObjects.Money(412);
            decimal discountPercent = 50;
            InvoiceItem item = new InvoiceItem(serviceId, quanity, serviceTitle, unitPrice, discountPercent);

            decimal expected_TotalPrice = 6 * 412;
            decimal expected_FinalPrice = 6 * 412 - (6 * 412 * 50) / 100;

            Assert.NotNull(item);
            Assert.Equal(serviceId, item.ServiceId);
            Assert.Equal(serviceTitle, item.ServiceTitle);
            Assert.Equal(quanity, item.Quantity);
            Assert.Equal(unitPrice.Amount, item.UnitPrice.Amount);
            Assert.Equal(unitPrice.Currency, item.UnitPrice.Currency);
            Assert.Equal(expected_TotalPrice, item.TotalPrice.Amount);
            Assert.Equal(expected_FinalPrice, item.FinalPrice.Amount);
            Assert.Equal(unitPrice.Currency, item.TotalPrice.Currency);
            Assert.Equal(unitPrice.Currency, item.FinalPrice.Currency);
        }
    }
}
