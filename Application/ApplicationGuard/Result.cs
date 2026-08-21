using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationGuard
{
    public class Result 
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public static Result Success() => new Result { IsSuccess = true};
        public static Result Faliure(string message) => new Result { IsSuccess = false, ErrorMessage = message };
    }
}
