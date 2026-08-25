using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utilities;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher> _mockHasher;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly IJwtservice _jwtService;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHasher = new Mock<IPasswordHasher>();
            _mockCustomerService = new Mock<ICustomerService>();

            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

            _jwtService = new JwtService(configuration);

            _service = new AuthService(
                _mockUserRepository.Object,
                _mockHasher.Object , 
                _mockUnitOfWork.Object,
                _jwtService ,
                _mockCustomerService.Object
            );
        }

        public class RegisterAsyncTests : AuthServiceTests
        {
            public class RegisterDtoTestClass : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    yield return new object[] { null };
                    yield return new object[] { new RegisterDto { Username = "", Password = "erfan335133", Phone = "09960357263", FullName = "efmirnfunf3u", Email = "ercmdcpm@gmail.com" } };
                    yield return new object[] { new RegisterDto { Username = "erfan", Password = "", Phone = "09960357263", FullName = "efmirnfunf3u", Email = "ercmdcpm@gmail.com" } };
                    yield return new object[] { new RegisterDto { Username = "erfan", Password = "erfan335133", Phone = "", FullName = "efmirnfunf3u", Email = "ercmdcpm@gmail.com" } };
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            [Theory]
            [ClassData(typeof(RegisterDtoTestClass))]
            public async Task Should_Return_Failure_When_Dto_Is_Null_Or_Some_Properties_Are_Invalid(RegisterDto dto)
            {
                Result result = await _service.RegisterAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal("ورودی نا معتبر است", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_User_Exists()
            {
                string mobile = "09960357263";
                string username = "erfan335133";
                User user = new User(username, "dclkjnlvfvcrn", mobile, Domain.Enums.RoleEnum.Customer);
                _mockUserRepository.Setup(x => x.GetByMobileOrUsernameAsync(username, mobile)).ReturnsAsync(user);

                RegisterDto dto = new RegisterDto
                {
                    Username = username,
                    Password = "fdbnruhgf7rgf74t",
                    Phone = mobile,
                    FullName = "Erfan ghorbani",
                    Email = "egmdidne@gmail.com"
                };
                Result result = await _service.RegisterAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal("چنین کاربری با این نام کاربری یا شماره تلفن قبلا ثبت شده است .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Add_User_When_All_Conditions_Are_Pass()
            {
                RegisterDto dto = new RegisterDto
                {
                    Username = "Erfan335133",
                    Password = "exubb3cygrcyr",
                    Phone = "09363651032",
                    FullName = "efmirnfunf3u",
                    Email = "ercmdcpm@gmail.com"
                };

                _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("inurcntcbyyy4cbryr");

                Result result = await _service.RegisterAsync(dto);

                Assert.True(result.IsSuccess);
                _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class RegisterAdminAsyncTests : AuthServiceTests
        {
            [Fact]
            public async Task Should_Return_Success_And_Add_User_When_All_Conditions_Are_Pass()
            {
                RegisterDto dto = new RegisterDto
                {
                    Username = "Erfan335133",
                    Password = "exubb3cygrcyr",
                    Phone = "09363651032",
                    FullName = "efmirnfunf3u",
                    Email = "ercmdcpm@gmail.com"
                };

                _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("inurcntcbyyy4cbryr");

                Result result = await _service.RegisterAsync(dto);

                Assert.True(result.IsSuccess);
                _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class LoginAsyncTests : AuthServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_Model_Is_Null()
            {
                Result<LoginResultDto> result = await _service.LoginAsync(null);

                Assert.False(result.IsSuccess);
                Assert.Equal("ورودی نا معتبر است .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_User_Was_Not_Founded()
            {
                LoginDto model = new LoginDto
                {
                    UserName = "Erfan335133",
                    Password = "Erfan335133",
                };

                _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                Result<LoginResultDto> result = await _service.LoginAsync(model);
                Assert.False(result.IsSuccess);
                Assert.Equal("نام کاربری یا کلمه عبور اشتباه است .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_Verfiy_Hashed_Password_Was_Failed()
            {
                string userHashedPassword = "kdieubyr3brhcbr3yur";
                string EntryHashedPassword = "xmicrmcjrnchjchrcsr";
                _mockHasher.Setup(x => x.VerifyHashedPassword(userHashedPassword, EntryHashedPassword))
                    .Returns(PasswordVerificationResult.Failed);

                LoginDto dto = new LoginDto
                {
                    UserName = "Erfan335133",
                    Password = "Erfan335133"
                };
                User user = new User("Erfan335133", "Erfan335133", "09960357263", Domain.Enums.RoleEnum.Customer);
                _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);

                Result<LoginResultDto> result = await _service.LoginAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal("نام کاربری یا کلمه عبور اشتباه است .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_When_All_Conditions_Are_True()
            {
                _mockHasher.Setup(x => x.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(PasswordVerificationResult.Success);

                LoginDto dto = new LoginDto
                {
                    UserName = "Erfan335133",
                    Password = "Erfan335133"
                };
                User user = new User("Erfan335133", "Erfan335133", "09960357263", Domain.Enums.RoleEnum.Customer);
                _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);

                Result<LoginResultDto> result = await _service.LoginAsync(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class LoginWithMobileAsyncTests : AuthServiceTests
        {
            [Fact]
            public async Task Should_Return_Success_When_All_Conditions_Are_True()
            {
                _mockHasher.Setup(x => x.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(PasswordVerificationResult.Success);

                LoginWithMobileDto dto = new LoginWithMobileDto
                {
                    Phone = "09960357263",
                    Password = "Erfan335133"
                };
                User user = new User("Erfan335133", "Erfan335133", "09960357263", Domain.Enums.RoleEnum.Customer);
                _mockUserRepository.Setup(x => x.GetByMobileAsync(It.IsAny<string>())).ReturnsAsync(user);

                Result<LoginResultDto> result = await _service.LoginWithMobileAsync(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }

        }

        public class RefreshTokenAsyncTests : AuthServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_User_Was_Not_Founded()
            {
                _mockUserRepository.Setup(x => x.GetByRefreshToken(It.IsAny<string>())).ReturnsAsync((User)null);

                Result<RefreshTokenRespondDto> result = await _service.RefreshTokenAsync("deimienxu3 x");

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);

                Assert.Equal("کاربر مورد نظر یافت نشد .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_RefreshToken_Was_Expired()
            {
                User user = new User(
                    "erfan335133",
                    "dcncbhr jcrh",
                    "09960357263",
                    Domain.Enums.RoleEnum.Customer
                );

                string refreshTOken = "cnufnhbvyhbyhpedl0d40d;dpe0o49i9";

                user.SetRefreshToken(refreshTOken, DateTime.UtcNow.AddSeconds(1));
                _mockUserRepository.Setup(x => x.GetByRefreshToken(It.IsAny<string>())).ReturnsAsync(user);

                Thread.Sleep(5000);

                Result<RefreshTokenRespondDto> result = await _service.RefreshTokenAsync(refreshTOken);

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal("ورود ناموفق بود لطفا لاگین کنید.", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Success_And_New_Access_And_Refresh_Token_When_All_Conditins_Pass()
            {
                User user = new User(
                    "erfan335133",
                    "dcncbhrjcrhex2ex2e2x",
                    "09960357263",
                    Domain.Enums.RoleEnum.Customer
                );

                string refreshTOken = "cnufnhbvyhbyhpedl0d40d;dpe0o49i9";

                user.SetRefreshToken(refreshTOken, DateTime.UtcNow.AddDays(20));
                _mockUserRepository.Setup(x => x.GetByRefreshToken(It.IsAny<string>())).ReturnsAsync(user);

                Result<RefreshTokenRespondDto> result = await _service.RefreshTokenAsync(refreshTOken);

                Console.WriteLine("This is token : " + result.Value.AccessToken);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.NotEmpty(result.Value.AccessToken);
                Assert.NotNull(result.Value.AccessToken);
                Assert.NotEmpty(result.Value.RefreshToken);
                Assert.NotNull(result.Value.RefreshToken);
                Assert.True(result.Value.ExpireIn > 0);
            }
        }
    }
}
