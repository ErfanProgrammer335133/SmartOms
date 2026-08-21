using Application.Interfaces;
using Azure.Core;
using Dmain.ValueObjects;
using Infrastructure.InfraExceptions;
using Infrastructure.InfraGuard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly string _apiKey;
        private readonly int _templateId;
        private readonly HttpClient _httpClientp;

        public SmsService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["SmsSettings:Key"];
            _templateId = int.Parse(configuration["SmsSettings:templateId"]);
            _httpClientp = httpClient;
        }
        public async Task SendAsync(Phone phone, string code)
        {
            string url = "https://api.sms.ir/v1/send/verify";
            _httpClientp.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            VerifySendModel body = new VerifySendModel()
            {
                Mobile = phone.PhoneNumber,
                TemplateId = _templateId,
                Parameters = new VerifySendParameterModel[] {
                new VerifySendParameterModel {
                    Name = "Code", Value = code
                    }
                }
            };

            string jsonBody = JsonSerializer.Serialize(body);
            StringContent stringContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClientp.PostAsync(url, stringContent);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new SendSmsFailedException(result);
        }
    }
}
