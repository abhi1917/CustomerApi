using CustomerDA.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TestFunction
{
    public static class CustomerEventListner
    {
        [FunctionName("CustomerEventListner")]
        public static void Run([EventHubTrigger("%EventHubName%", Connection = "EventHub", ConsumerGroup = "$Default")] EventData[] events, TraceWriter log)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                foreach (EventData eventData in events)
                {
                    string value = "";
                    using (var reader = new StreamReader(eventData.GetBodyStream(), Encoding.UTF8))
                    {
                        value = reader.ReadToEnd();
                        CustomerSendEvent customer = new CustomerSendEvent
                        {
                            CustomerId = value,
                            AgentId = "EventHub"
                        };
                        var url = System.Environment.GetEnvironmentVariable("apiEventInvokeurl");
                        var content = JsonConvert.SerializeObject(customer);
                        if (null != content)
                        {
                            var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", System.Environment.GetEnvironmentVariable("apiAuthToken"));
                            var result = httpClient.PostAsync(url, stringContent);
                        }
                        else
                        {
                            throw new Exception("Failed to serialize object!");
                        }
                    }
                    log.Info(value);
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
        }
    }
}
