using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CustomerDA.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace TestFunction
{
    public static class ChangeFeedListnerTest
    {
        [FunctionName("ChangeFeedListnerTest")]
        public static void Run([CosmosDBTrigger(
            databaseName: "%databaseId%",
            collectionName: "%containerId%",
            ConnectionStringSetting = "AzureconnectionString",
            LeaseCollectionName = "leases",CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> documents, TraceWriter log)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                if (documents != null && documents.Count > 0)
                {
                    log.Info(documents[0].ToString());
                    log.Verbose("Documents modified " + documents.Count);
                    
                    CustomerSendEvent customer = new CustomerSendEvent
                    {
                        CustomerId = documents[0].Id,
                        AgentId = "ChangeFeed"
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
            }catch(Exception ex)
            {
                log.Error(ex.Message);
            }
        }
    }
}
