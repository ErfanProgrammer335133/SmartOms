using Application.ApplicationGuard;
using Application.Exceptions;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class ServiceHelper
    {
        public static async Task<Result<T>> Do<T>
            (Func<Task<Result<T>>> func, int maxRetry , Func<PaymentTransaction>? concurrencyFunc = null)
        {
            int retry = maxRetry;
            while(retry > 0)
            {
                try
                {
                    return await func();
                }
                catch (NotFoundException ex)
                {
                    return Result<T>.Failure(ex.Message);
                }
                catch (DomainValidationException ex)
                {
                    return Result<T>.Failure(ex.Message);
                }
                catch (MoneyValidationException ex)
                {
                    return Result<T>.Failure(ex.Message);
                }
                catch (DatabaseConcurrencyException)
                {
                    if (concurrencyFunc is not null)
                        concurrencyFunc();
                    retry--;
                    if (retry == 0)
                        return Result<T>.Failure("خطای همزمانی رخ داده است. لطفا مجدادا تلاش کنید.");
                }
            }

            return Result<T>.Failure("خطای غیر منتظره ای رخ داده است . unknown error");
        }
        public static async Task<Result> Do(Func<Task<Result>> func, int maxRetry)
        {
            int retry = maxRetry;
            while(retry > 0)
            {
                try
                {
                    return await func();
                }
                catch (NotFoundException ex)
                {
                    Console.WriteLine("Not found erreo");
                    return Result.Faliure(ex.Message);
                }
                catch (DomainValidationException ex)
                {
                    Console.WriteLine("domain error");
                    return Result.Faliure(ex.Message);
                }
                catch (MoneyValidationException ex)
                {
                    Console.WriteLine("Money error");
                    return Result.Faliure(ex.Message);
                }
                catch (DatabaseConcurrencyException)
                {
                    Console.WriteLine("Concurrency error");
                    retry--;
                    if (retry == 0)
                        return Result.Faliure("خطای همزمانی رخ داده است. لطفا مجدادا تلاش کنید.");
                }
            }

            return Result.Faliure("خطای غیر منتظره ای رخ داده است .");
        }

        public static async Task<T?> GetEntityFromRepo<T>(Func<Task<T?>> func , string errorMessage) 
            where T : class
        {
            var entity = await func();
            if (entity is null)
                throw new NotFoundException(errorMessage);
            return entity;
        }
    }
}
