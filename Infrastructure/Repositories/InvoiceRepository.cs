using Dmain.Entities;
using Dmain.IRepositories;
using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Database_Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice> , IInvoiceRepository
    {
        public InvoiceRepository(Context context) : base(context)
        {
            
        }
    }
}
