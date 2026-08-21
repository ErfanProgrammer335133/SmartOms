
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Phone
    {
        public string PhoneNumber { get; private set; }

        public Phone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new PhoneValidationException("شماره تلفن نمیتواند خالی باشد.");

            if(!phone.All(char.IsDigit))
                throw new PhoneValidationException
                    ("شماره تلفن فقط باید تشکیل شده از ارقام باشد و نمی تواند شامل حروف شود.");

            if (!phone.StartsWith("09") && !phone.StartsWith("98") && !phone.StartsWith("+98"))
                throw new PhoneValidationException("شماره تلفن معتبر نمی باشد .");

            if ((phone.StartsWith("09") && phone.Length != 11) ||
                (phone.StartsWith("98") && phone[2] != '9' && phone.Length != 12) ||
                (phone.StartsWith("+98") && phone[3] != '9' && phone.Length != 13)
                )
                throw new PhoneValidationException("تعداد ارقام معتبر نمیباشد .");

            string NormalizedPhone = Normalize(phone);
            if (!IsValid(NormalizedPhone))
                throw new PhoneValidationException("فرمت شماره تلفن نامعتبر است");

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

        public override bool Equals(object? obj)
        {
            return obj is Phone other && other.PhoneNumber == this.PhoneNumber;
        }
    }
}
