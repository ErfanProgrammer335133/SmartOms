using Dmain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; private set; }
        public bool IsActive { get; private set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
            IsActive = true;
        }

        public void Activate()
        {
            IsActive = true; ;
        }
        public void DeActivate()
        {
            IsActive = false; ;
        }
    }
}
