using Dmain.Enums;
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
    public class BusinessService : BaseEntity
    {
        public string Title { get; private set; }
        public string Explanation { get; private set; }
        public Money Price { get; private set; }

        public BusinessService(string title, string explanation, Money price)
        {
            SetTitle(title);
            SetExplenation(explanation);
            SetPrice(price);
        }

        public void SetTitle(string title)
        {
            Guard.CommonValidations(title, 1, 100,
                "عنوان نمی تواند خالی باشد", "طول عنوان نمی تواند بیشتر از 100 کارکتر باشد.");
            Title = title;
        }

        public void SetExplenation(string explenation)
        {
            Guard.CommonValidations(explenation, 20, 500,
                "توضیحات نمی تواند خالی باشد", "توضیحات حداقل 20 کارکتر و حداکثر 500 کارکتراست.");
            Explanation = explenation;
        }

        private void SetPrice(Money price)
        {
            if (price.Amount < 0)
                throw new MoneyValidationException("قیمت نمی تواند منفی باشد.");
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
            if (amount < 0)
                throw new MoneyValidationException("قیمت نمی‌تواند منفی شود.");
            Money newPrice = new Money(amount, Price.Currency);
            SetPrice(newPrice);
        }
    }
}
