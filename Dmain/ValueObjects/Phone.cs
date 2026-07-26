
using Dmain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.ValueObjects
{
    public class Phone
    {
        public string PhoneNumber { get; private set; }

        public Phone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainValidationException("شماره تلفن نمیتواند خالی باشد.");

            string NormalizedPhone = Normalize(phone);
            if (!IsValid(NormalizedPhone))
                throw new DomainValidationException("فرمت شماره تلفن نامعتبر است");

            PhoneNumber = NormalizedPhone;
        }

        private string Normalize(string phone)
        {
            phone = phone.Replace(" ", "");
            if (phone.StartsWith("09"))
                return "98" + phone.Substring(1);
            if (phone.StartsWith("+98"))
                return phone.Substring(1);
            return phone;
        }

        public string ToLocal(string normalPhone) => "0" + normalPhone.Substring(2);

        private bool IsValid(string phone) =>
            phone.Length == 12 && phone.StartsWith("989") && phone.All(char.IsDigit);
    }
}
