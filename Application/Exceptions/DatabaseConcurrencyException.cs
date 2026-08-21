using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class DatabaseConcurrencyException :Exception
    {
        public DatabaseConcurrencyException()
        {
            
        }

        public DatabaseConcurrencyException(string message) : base(message)
        {
            
        }
        
        public DatabaseConcurrencyException(string message, Exception inner) : base(message , inner)
        {
            
        }
    }
}
