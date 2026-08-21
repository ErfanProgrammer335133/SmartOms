using Domain.Enums;
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
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string HashPassword { get; private set; }
        public Phone Phone { get; private set; }
        public RoleEnum Role { get; private set; }
        public bool IsVerified { get; private set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }

        public User(string username , string hashPassword , string phone , RoleEnum role)
        {
            SetUsername(username);
            SetPassword(hashPassword);
            Role = role;
            Phone = new Phone(phone);
            IsVerified = false;
        }

        public void Verify() => IsVerified = true;

        public void SetUsername(string username)
        {
            TextValidationGuard.CommonValidations(username, 8, 100, 
                "نام کاربری نمی تواند خالی باشد", "نام کاربری باید بین 8 تا 100 کارکتر باشد .");

           Username = username;
        }
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainValidationException("رمز عبور نمی تواند خالی باشد");

            HashPassword = password;
        }

        public void SetRefreshToken(string token , DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainValidationException("توکن نا معتبر است.");
            if (expiry <= DateTime.UtcNow)
                throw new DomainValidationException("زمان انقضای توکن نا معتبر است.");

            RefreshToken = token;
            Expiry = expiry;
        }

        public void SetPhone(string phone)
        {
            Phone newPhone = new Phone(phone);

            if (newPhone.Equals(Phone))
                throw new PhoneValidationException("شماره موبایل جدید با قبلی برابر است");

            Phone = newPhone;
        }


        public void SetRole(RoleEnum role)
        {
            Role = role;
        }

    }
}
