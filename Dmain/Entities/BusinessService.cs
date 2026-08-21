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
    public class BusinessService : BaseEntity
    {
        public string Title { get; private set; }
        public string Explanation { get; private set; }
        public Money Price { get; private set; }
        public int MaxQuantity { get; private set; }
        public bool IsFree { get; private set; }

        public BusinessService(string title, string explanation, Money price , int maxQuantity, bool isFree)
        {
            IsFree = isFree;
            SetTitle(title);
            SetExplenation(explanation);
            SetPrice(price);
            if (maxQuantity <= 0)
                throw new DomainValidationException("تعداد نمی تواند منفی یا صف باشد");
            MaxQuantity = maxQuantity;
        }

        public void SetTitle(string title)
        {
            TextValidationGuard.CommonValidations(title, 1, 100,
                "عنوان نمی تواند خالی باشد", "طول عنوان نمی تواند بیشتر از 100 کارکتر باشد.");
            Title = title;
        }

        public void SetExplenation(string explenation)
        {
            TextValidationGuard.CommonValidations(explenation, 20, 500,
                "توضیحات نمی تواند خالی باشد", "توضیحات حداقل 20 کارکتر و حداکثر 500 کارکتراست.");
            Explanation = explenation;
        }

        public void SetPrice(Money price)
        {
            if (price.Amount < 0)
                throw new MoneyValidationException("قیمت نمی تواند منفی باشد.");
            if(price.Amount == 0 && !IsFree)
                throw new MoneyValidationException("این سرویس یک سرویس رایگان نمی باشد.");
            if(IsFree && price.Amount != 0)
                throw new MoneyValidationException("این یک سرویس رایگان است و قیمت ان باید صفر باشد.");

            Price = price;
        }

        public void IncreasePrice(decimal percent)
        {
            if (percent <= 0 || percent > 100)
                throw new MoneyValidationException("درصد افزایش قیمت باید بین 0 تا 100 درصد باشد.");
            decimal amount = Price.Amount + (Price.Amount * percent) / 100;
            Money newPrice = new Money(amount, Price.Currency);
            SetPrice(newPrice);
        }
        
        public void DecreasePrice(decimal percent)
        {
            if (percent <= 0 || percent > 100)
                throw new MoneyValidationException("درصد کاهش قیمت باید بین 0 تا 100 درصد باشد.");
            decimal amount = Price.Amount - (Price.Amount * percent) / 100;
            if(percent == 100 && !IsFree)
                throw new MoneyValidationException("قیمت به صفر رسیده در حالی که این یک سرویس رایگان نیست.");
            if (amount < 0)
                throw new MoneyValidationException("قیمت نمی‌تواند منفی شود.");
            Money newPrice = new Money(amount, Price.Currency);
            SetPrice(newPrice);
        }

        public void SetFree()
        {
            IsFree = true;
            Price = new Money(0);
        }

        public void SetNotFree(Money price)
        {
            if (price.Amount == 0)
                throw new MoneyValidationException
                    ("برای پولی کردن این سرویس باید قیمت را بیشتر از صفر وارد منید .");
            Price = price;
            IsFree = false;
        }
    }
}
