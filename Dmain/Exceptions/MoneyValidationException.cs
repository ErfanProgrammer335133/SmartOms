using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class MoneyValidationException : Exception
    {
        public MoneyValidationException()
        {
            
        }

        public MoneyValidationException(string message) : base(message)
        {
            
        } 
        
        public MoneyValidationException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
