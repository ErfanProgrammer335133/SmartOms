using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class PhoneTests
    {
        [Fact]
        public void Constructor_Should_Throw_PhoneValidationException_When_PhoneNumber_Is_Empty()
        {
            Assert.Throws<PhoneValidationException>(() => new Phone(""));
        }

        [Theory]
        [InlineData("")]
        [InlineData("78965412365")]
        [InlineData("0912365478952")]
        [InlineData("9891236552")]
        [InlineData("9891kj36552")]
        public void Constructor_Should_Throw_PhoneValidationException_When_PhoneNumber_Is_Invalid(string phoneNumber)
        {
            Assert.Throws<PhoneValidationException>(() => new Phone(phoneNumber));
        }

        [Fact]
        public void Constructor_Should_Create_Phone_Correctly_If_Phone_Is_Valid_In_Normal_Format()
        {
            Phone phone = new Phone("09363651032");
            string normalForm = "989363651032";

            Assert.NotNull(phone);
            Assert.Equal(normalForm, phone.PhoneNumber);
        }

        [Fact]
        public void ToLocal_Should_Convert_Normal_Form_To_Local_Form()
        {
            string localForm = "09363651032";
            Phone phone = new Phone(localForm);

            Assert.Equal(localForm, phone.ToLocal(phone.PhoneNumber));
        }
    }
}
