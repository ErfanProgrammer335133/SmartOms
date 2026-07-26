using Dmain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.ValueObjects
{
    public class Email
    {
        public string Address { get; private set; }

        public Email(string address)
        {
            IsEmailValid(address);
            Address = address;
        }

        public void IsEmailValid(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainValidationException("ایمیل نمی تواند خالی باشد .");
            if (!
                (address.Contains('@')
                && (address.EndsWith(".ir") || address.EndsWith(".com") || address.EndsWith(".org"))))
                throw new DomainValidationException("ایمیل نا معتبر اسشت");
        }
    }
}
