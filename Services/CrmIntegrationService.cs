using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApi.Services
{
    public class CrmIntegrationService
    {
        public static readonly HttpClient client = new HttpClient();

        private const string WebhookUrl = "https://b24-5vw10r.bitrix24.ru/rest/1/ne25v7juv7mg7xhe/crm.contact.add.json";

        public async Task SendContactToCrm(string name, string email, bool notifications, string status, int income)
        {
            var contactData = new
            {
                fields = new
                {
                    NAME = name,
                    EMAIL = new[] { new { VALUE = email, VALUE_TYPE = "WORK" } },
                    // Используй свои реальные ID полей (UF_CRM_...)
                    UF_CRM_1777801523181 = notifications ? "Y" : "N",
                    UF_CRM_1777801592736 = status, // Если это список, тут должен быть ID значения
                    UF_CRM_1777734088995 = income
                }
            };

            string json = JsonConvert.SerializeObject(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(WebhookUrl, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"[CRM Service Error]: {ex.ToString()}");
            }
        }
    }
}
