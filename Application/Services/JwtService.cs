using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
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
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;

        public JwtService(IConfiguration configuration)
        {
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _key = configuration["Jwt:Key"];
        }
        public string GenerateToken(User user)
        {
            if (user == null)
                throw new NullReferenceException("ورودی نامعتبر ... ");

            Claim[] Claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.Name , user.Username),
                new Claim(ClaimTypes.Role , user.Role == RoleEnum.Customer ? "Customer" : "Admin"),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            SigningCredentials signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken
                (
                issuer: _issuer,
                audience: _audience,
                signingCredentials: signing,
                claims: Claims,
                expires: DateTime.UtcNow.AddHours(1)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
