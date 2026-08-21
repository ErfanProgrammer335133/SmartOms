using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OrderStatusEnum
    {
        Pending =  0, 
        Paid = 1 , 
        InProgress = 2,
        Completed = 3, 
        Canceled = 4
    }
}
