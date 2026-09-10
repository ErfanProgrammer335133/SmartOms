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
    public class CustomerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_Should_Throw_DomainValidationException_When_FullName_Is_Null_Or_Empty(string fullname)
        {
            Assert.Throws<DomainValidationException>(() =>
            new Customer(Guid.NewGuid(), fullname, "egho@gmail.com"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678910")]
        [InlineData("0936365103")]
        [InlineData("98936365103")]
        [InlineData("+9893636510321")]
        [InlineData("0936365103u")]
        public void Constructor_Should_Throw_PhoneValidationException_When_Phone_Is_Invalid(string phone)
        {
            Assert.Throws<PhoneValidationException>(() =>
                new Customer(Guid.NewGuid(), "test fullname", "test@gmail.com"));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("erfanemail")]
        [InlineData("test.com")]
        [InlineData("test@")]
        public void Constructor_Should_Throw_EmailValidationException_When_Email_Is_Invalid(string email)
        {
            Assert.Throws<EmailValidationException>(() =>
                new Customer(Guid.NewGuid(), "test fullname",  email));
        }

        [Fact]
        public void Constructor_Should_Create_Customer_When_All_Parameters_Are_Correct()
        {
            Guid userId = Guid.NewGuid();
            string fullname = "test fullname";
            string email = "test@gmail.com";
            Customer customer = new Customer(userId, fullname, email);

            Assert.NotNull(customer);
            Assert.Equal(fullname, customer.FullName);
            Assert.Equal(email, customer?.Email?.Address);
        }

        [Fact]
        public void SetFullName_Should_Set_Customers_Name_Correctly()
        {
            Customer customer = new Customer(Guid.NewGuid(), "test fullname", "test@gmail.com");
            string expectedName = "Erfan ghorbani";
            customer.SetFullname(expectedName);

            Assert.Equal(expectedName, customer.FullName);
        }
        

        [Fact]
        public void SetPhone_Should_Throw_EmailValidationException_When_New_Email_Is_Equal_To_Previous_Email()
        {
            Customer customer = new Customer(Guid.NewGuid(), "test fullname", "test@gmail.com");

            Assert.Throws<EmailValidationException>(() => customer.SetEmail("test@gmail.com"));
        }

        [Fact]
        public void SetEmail_Should_Set_Email_When_Parameter_Is_Correct()
        {
            Customer customer = new Customer(Guid.NewGuid(), "test fullname", "test@gmail.com");
            Email expectedEmail = new Email("test2@gmail.com");
            customer.SetEmail("test2@gmail.com");

            Assert.True(expectedEmail.Equals(customer.Email));
        }
    }
}
