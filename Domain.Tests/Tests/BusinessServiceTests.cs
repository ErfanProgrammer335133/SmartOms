using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class BusinessServiceTest
    {
        [Fact]
        public void Should_Throw_DomainValidationException_When_Title_Is_Empty()
        {
            Assert.Throws<DomainValidationException>(() => new BusinessService(
                title: "",
                explanation: "Test explanation and should be more than 20 character",
                new ValueObjects.Money(250),
                3,
                isFree: false
                ));
        }

        [Theory]
        [InlineData("")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "a")]
        public void Should_Throw_DomainValidationException_When_Explanation_Is_Empty_Or_More_Than500(string explanation)
        {
            Assert.Throws<DomainValidationException>(() => new BusinessService(
                title: "test title",
                explanation: explanation,
                new ValueObjects.Money(250),
                3,
                isFree: false
                ));
        }

        [Fact]
        public void Should_Throw_DomainValidationException_When_MaxQuantity_Is_Equal_Or_Less_Than0()
        {
            Assert.Throws<DomainValidationException>(() => new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(250),
               0,
                isFree: false
               ));
        }

        [Fact]
        public void Should_Throw_MoneyValidationException_When_Price_Is_Less_Than0()
        {
            Assert.Throws<MoneyValidationException>(() => new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(-1),
               2,
                isFree: false
               ));
        }

        [Fact]
        public void Should_Throw_MoneyValidationException_When_Price_Is_0_And_IsFree_Is_False()
        {
            Assert.Throws<MoneyValidationException>(() => new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(0),
               2,
                isFree: false
               ));
        }

        [Fact]
        public void Should_Throw_MoneyValidationException_When_IsFree_Is_True_But_Price_IsNot0()
        {
            Assert.Throws<MoneyValidationException>(() => new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(1),
                2,
                isFree: true
               ));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Should_Throw_MoneyValidationException_When_IncreasePercent_Is_Greater_Than100_Or_Less_Than0(decimal percent)
        {
            BusinessService service = new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(150),
                2,
                isFree: false
               );

            Assert.Throws<MoneyValidationException>(() => service.IncreasePrice(percent));
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Should_Throw_MoneyValidationException_When_DecreasePercent_Is_Greater_Than100_Or_Less_Than0(decimal percent)
        {
            BusinessService service = new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(150),
                2,
                isFree: false
               );

            Assert.Throws<MoneyValidationException>(() => service.DecreasePrice(percent));
        }

        [Fact]
        public void Should_Throw_MoneyValidationException_When_DecreasePercent_Is_100_And_IsFree_Is_False()
        {
            BusinessService service = new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(150),
                2,
                isFree: false
               );

            Assert.Throws<MoneyValidationException>(() => service.DecreasePrice(100));
        }

        [Fact]
        public void IncreasePrice_Should_Increase_Price_correctly()
        {
            BusinessService service = new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(100),
                2,
                isFree: false
               );

            decimal IncreasePercent = 10;
            decimal expectedAmout = 110;
            service.IncreasePrice(IncreasePercent);

            Assert.Equal(expectedAmout , service.Price.Amount);
        }

        [Fact]
        public void DecreasePrice_Should_Decrease_Price_correctly()
        {
            BusinessService service = new BusinessService(
               title: "test title",
               explanation: "Test explanation and should be more than 20 character",
               new ValueObjects.Money(100),
                2,
                isFree: false
               );

            decimal decreasePercent = 10;
            decimal expectedAmout = 90;
            service.DecreasePrice(decreasePercent);

            Assert.Equal(expectedAmout , service.Price.Amount);
        }

        [Fact]
        public void Constructor_Should_Create_Service_With_Valid_Data()
        {
            var service = new BusinessService(
                title: "Valid Title",
                explanation: "This is a valid explanation with more than 20 characters.",
                price: new ValueObjects.Money(250),
                maxQuantity: 3,
                isFree: false
            );

            Assert.Equal("Valid Title", service.Title);
            Assert.Equal("This is a valid explanation with more than 20 characters.", service.Explanation);
            Assert.Equal(250, service.Price.Amount);
            Assert.Equal(3, service.MaxQuantity);
            Assert.False(service.IsFree);
        }
    }
}
