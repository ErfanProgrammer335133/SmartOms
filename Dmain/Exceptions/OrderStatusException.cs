using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Exceptions
{
    public class OrderStatusException : Exception
    {
        public OrderStatusException()
        {
            
        }

        public OrderStatusException(string message) : base(message)
        {
            
        }
        
        public OrderStatusException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
