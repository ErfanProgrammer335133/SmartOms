using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public static class TextValidationGuard
    {
        public static void CommonValidations(
            string text, int minLength, int maxLength, string EmptyErrorMessage, string LengthErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new DomainValidationException(EmptyErrorMessage);
            if (text.Length < minLength || text.Length > maxLength)
                throw new DomainValidationException(LengthErrorMessage);
        }
    }
}
