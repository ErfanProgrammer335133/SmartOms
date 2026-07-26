using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Exceptions
{
    class DomainValidationException : Exception
    {
        public DomainValidationException()
        {

        }

        public DomainValidationException(string message) : base(message)
        {

        }

        public DomainValidationException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
