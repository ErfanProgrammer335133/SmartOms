using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class PaymentFailException : Exception
    {
        public PaymentFailException()
        {
            
        }

        public PaymentFailException(string message) : base(message)
        {
            
        }

        public PaymentFailException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
