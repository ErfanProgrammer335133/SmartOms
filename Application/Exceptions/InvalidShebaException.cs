using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class InvalidShebaException : Exception
    {
        public InvalidShebaException()
        {
            
        }

        public InvalidShebaException(string message) : base(message)
        {
            
        }

        public InvalidShebaException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
