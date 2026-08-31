using Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities
{
    public static class ValidateSheba
    {
        public static void Validate(string sheba)
        {
            if (string.IsNullOrWhiteSpace(sheba))
                throw new InvalidShebaException("شماره شبا خالی است .");

        }
    }
}
