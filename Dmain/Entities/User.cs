using Dmain.Enums;
using Dmain.Exceptions;
using Dmain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string HashPassword { get; private set; }
        public RoleEnum Role { get; private set; }

        public User(string username , string hashPassword , RoleEnum role)
        {
            SetUsername(username);
            SetPassword(hashPassword);
            Role = role;
        }

        public void SetUsername(string username)
        {
            Guard.CommonValidations(username, 8, 100, 
                "نام کاربری نمی تواند خالی باشد", "نام کاربری باید بین 8 تا 100 کارکتر باشد .");

           Username = username;
        }
        public void SetPassword(string password)
        {
            Guard.CommonValidations(password, 8, 100,
                "رمز عبور نمی تواند خالی باشد", "رمز عبور باید بین 8 تا 100 کارکتر باشد .");

            HashPassword = password;
        }

        public void SetRole(RoleEnum role)
        {
            Role = role;
        }

    }
}
