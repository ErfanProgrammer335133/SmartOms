using Application.Services;
using Application.Utilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _service;

        public JwtServiceTests(JwtService service)
        {
            _service = service;
        }
        [Fact]
        public void Should_Return_True()
        {
            bool res = true;
            Assert.True(res);
        }
    }
}
