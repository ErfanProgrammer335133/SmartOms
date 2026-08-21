using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared
{
    public static class RowVersionGuard
    {
        public static void AddRowVersionToAll(ModelBuilder builder , List<Type> tables)
        {
            foreach(var table in tables)
                builder.Entity(table).Property("RowVersion").IsRowVersion();
        }
    }
}
