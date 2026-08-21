using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
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
                throw new EmailValidationException("ایمیل نمی تواند خالی باشد .");
            if (!
                (address.Contains('@')
                && (address.EndsWith(".ir") || address.EndsWith(".com") || address.EndsWith(".org"))
                && !address.Trim().Contains(" ")
                ))
                throw new EmailValidationException("ایمیل نا معتبر است");
        }

        public override bool Equals(object? obj)
        {
            return obj is Email other && other.Address == this.Address;
        }
    }
}
