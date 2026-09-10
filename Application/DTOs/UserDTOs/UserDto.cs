using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string HashPassword { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}
