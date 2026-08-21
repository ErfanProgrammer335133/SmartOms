using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    class PaymentFailureException : Exception
    {
        public PaymentFailureException()
        {
            
        }

        public PaymentFailureException(string message) : base(message)
        {
            
        }
        
        public PaymentFailureException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
