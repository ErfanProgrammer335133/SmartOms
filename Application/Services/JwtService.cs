using Application.Interfaces;
using Application.Utilities;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class JwtService : IJwtservice
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
        }
        public string GenerateToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "ورودی نامعتبر است");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? "Unknown"),
                new Claim(ClaimTypes.Role, user.Role == RoleEnum.Customer ? "Customer" : "Admin"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Claims = claims.ToDictionary(c => c.Type, c => (object)c.Value),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signing
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
