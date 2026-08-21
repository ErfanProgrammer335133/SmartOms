using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class EmailValidationException : Exception
    {
        public EmailValidationException()
        {

        }

        public EmailValidationException(string message) : base(message)
        {

        }

        public EmailValidationException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
