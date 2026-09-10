using Domain.Exceptions;
using Domain.Shared;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string FullName { get; private set; }
        public Email? Email { get; private set; }

        public Customer(Guid userId , string fullName , string email)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            SetFullname(fullName);
            if(!string.IsNullOrEmpty(email))
                Email = new Email(email);
        }

        private Customer()
        {

        }

        public void SetFullname(string fullName)
        {
            TextValidationGuard.CommonValidations(fullName, 5, 100,
                "نام و نام خانوادگی نمی تواند خالی باشد",
                "نام و نام خانوادگی باید حداقل 5 و حداکثر 100 کارکتر باشد .");
            if (fullName == FullName)
                throw new DomainValidationException("نام جدید با نام فعلی برابر است");

            FullName = fullName;
        }

        public void SetEmail(string email)
        {
            Email newEmail = new Email(email);

            if (Email != null && newEmail.Equals(Email))
                throw new EmailValidationException("ایمیل جدید با قبلی برابر است");

            Email = newEmail;
        }

    }
}
