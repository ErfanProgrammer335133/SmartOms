using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class EmailTests
    {
        [Fact]
        public void IsValid_Should_Throw_EmailValidationException_When_Address_Is_Empty()
        {
            Assert.Throws<EmailValidationException>(() => new Email(""));
        }

        [Theory]
        [InlineData("ek mnckn@wi.com")]
        [InlineData("email.ir")]
        [InlineData("email@doj")]
        [InlineData("email@doj.pi")]
        public void IsValid_Should_Throw_EmailValidationException_When_Address_Is_Invalid(string address)
        {
            Assert.Throws<EmailValidationException>(() => new Email(address));
        }

        [Fact]
        public void Constructor_Should_Create_Email_When_Address_Is_Valid()
        {
            string address = "erfan@335133.com";
            Email email = new Email(address);

            Assert.NotNull(email);
            Assert.Equal(address, email.Address);
        }
    }
}
