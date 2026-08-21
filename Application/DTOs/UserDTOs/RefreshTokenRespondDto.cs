using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTOs
{
    public class RefreshTokenRespondDto
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public int ExpireIn { get; set; }
    }
}
