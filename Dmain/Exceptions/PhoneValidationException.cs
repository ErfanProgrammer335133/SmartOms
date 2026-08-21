using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class PhoneValidationException : Exception
    {
        public PhoneValidationException()
        {

        }

        public PhoneValidationException(string message) : base(message)
        {
            
        }

        public PhoneValidationException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
