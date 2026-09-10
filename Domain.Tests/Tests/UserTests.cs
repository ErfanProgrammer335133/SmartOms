using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class UserTests
    {
        [Theory]
        [InlineData("aaaaaaa")] // Length = 7 
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaav")] // Length = 101
        public void Constructor_Should_Throw_DomainValidationException_When_usernames_Length_Is_Less_Than8_Or_Greater_Than100(string username)
        {
            Assert.Throws<DomainValidationException>(() => new User(username, "erfan335133", "09960357263" , RoleEnum.Admin));
        }

        [Theory]
        [InlineData("aaaaaaa")] // Length = 7 
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaav")] // Length = 101
        public void Constructor_Should_Throw_DomainValidationException_When_Passwords_Length_Is_Less_Than8_Or_Greater_Than100(string password)
        {
            Assert.Throws<DomainValidationException>(() => new User("erfan", password, "09960357263", RoleEnum.Admin));
        }

        [Fact]
        public void Constructor_Should_Create_User_Correctly_When_All_Parameters_Are_Valid()
        {
            string username = "erfan335133";
            string password = "erfan335133";
            RoleEnum role = RoleEnum.Customer;

            User user = new User(username, password, "09960357263", role);

            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Equal(password, user.HashPassword);
            Assert.Equal(role, user.Role);
            Assert.False(user.IsVerified);
        }

        [Fact]
        public void Verify_Should_Change_IsVrified_To_True()
        {
            User user = new User("test user name", "test password", "09960357263", RoleEnum.Customer);
            user.Verify();
            Assert.True(user.IsVerified);
        }

        [Fact]
        public void SetRole_Should_Set_Role_Correctly()
        {
            User user = new User("test user name", "test password", "09960357263", RoleEnum.Customer);
            user.SetRole(RoleEnum.Admin);
            Assert.Equal(RoleEnum.Admin, user.Role);
        }
    }
}
