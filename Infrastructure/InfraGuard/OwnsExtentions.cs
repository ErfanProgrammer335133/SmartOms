using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared
{
    public static class OwnedProppertyExtentions
    {
        public static void OwnsMoney<T>(
            this EntityTypeBuilder<T> builder,
            Expression<Func<T, Money?>> propertyExpression,
            string prefix
        ) where T : class
        {
            builder.OwnsOne(propertyExpression, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName($"{prefix}_Amount")
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName($"{prefix}_Currency")
                    .IsRequired();
            });
        }
    }
}
