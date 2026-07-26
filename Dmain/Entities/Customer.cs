using Dmain.Exceptions;
using Dmain.Shared;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string FullName { get; private set; }
        public Phone Phone { get; private set; }
        public Email? Email { get; private set; }

        private readonly Wallet Wallet;

        public Customer(Guid userId , string fullName , string phone , string email)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            SetFullname(fullName);
            Phone = new Phone(phone);
            if(!string.IsNullOrWhiteSpace(email))
                Email = new Email(email);
            Wallet = new Wallet(Id);
        }

        public void SetFullname(string fullName)
        {
            Guard.CommonValidations(fullName, 5, 100,
                "نام و نام خانوادگی نمی تواند خالی باشد",
                "نام و نام خانوادگی باید حداقل 5 و حداکثر 100 کارکتر باشد .");
            if (fullName == FullName)
                throw new DomainValidationException("نام جدید با نام فعلی برابر است");

            FullName = fullName;
        }

        public void SetPhone(string phone)
        {
            var newPhone = new Phone(phone);

            if (newPhone.Equals(Phone))
                throw new DomainValidationException("شماره موبایل جدید با قبلی برابر است");

            Phone = newPhone;
        }

        public void SetEmail(string email)
        {
            var newEmail = new Email(email);

            if (Email != null && newEmail.Equals(Email))
                throw new DomainValidationException("ایمیل جدید با قبلی برابر است");

            Email = newEmail;
        }

    }
}
