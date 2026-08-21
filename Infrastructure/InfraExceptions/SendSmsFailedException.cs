using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.InfraExceptions
{
    class SendSmsFailedException : Exception
    {
        public SendSmsFailedException()
        {
            
        }

        public SendSmsFailedException(string message) : base(message)
        {
            
        }
        
        public SendSmsFailedException(string message , Exception inner) : base(message , inner)
        {
            
        }
    }
}
