using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationGuard
{
    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true , Value = value};  
        public static Result<T> Failure(string mesage) => new Result<T> { IsSuccess = false , ErrorMessage = mesage};  
    }
}
