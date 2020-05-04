using System;
using System.Collections.Generic;
using System.Net.Http;
using CustomerDA.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TestFunction
{
    public static class CosmosAddEvent
    {
        [FunctionName("CosmosAddEvent")]
        public static void Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, 
            [DocumentDB(
            databaseName: "%databaseId%",
            collectionName: "%EventContainer%",
            ConnectionStringSetting = "AzureconnectionString",CreateIfNotExists = true,PartitionKey ="%CosmosPartitionKey%")]out object customer, TraceWriter log)
        {
            try
            {
                customer = req.Content.ReadAsAsync<object>().Result;
                if (null != customer)
                {
                    log.Info("Insertion done");
                }
                else
                {
                    throw new Exception("Failed to serialize object!");
                }
            }
            catch(Exception ex)
            {
                customer = null;
                log.Error(ex.Message);
            }
        }
    }
}
