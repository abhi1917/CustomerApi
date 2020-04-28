using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerDA.Executions;
using CustomerDA.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace TestFunction
{
    public static class CustomerCosmosView
    {
        [FunctionName("CustomerCosmosView")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Customer View begins");
            string jsonValue = "";
            HttpResponseMessage response;
            try
            {
                string endpointUri = System.Environment.GetEnvironmentVariable("endpointUri");
                string primaryKey = System.Environment.GetEnvironmentVariable("customerPrimaryKey");
                string databaseId = System.Environment.GetEnvironmentVariable("databaseId");
                string containerId = System.Environment.GetEnvironmentVariable("containerId");
                CosmosDA cosmosDA = new CosmosDA(endpointUri, primaryKey, databaseId, containerId);
                if ((req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value != null) && (req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value != null))
                {
                    string id = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value.Replace("\"", "");
                    string lastName = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value.Replace("\"", "");
                    await cosmosDA.GetCustomerbyID(id,lastName);
                }
                else if (req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value != null)
                {
                    string lastName = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value.Replace("\"", "");
                    await cosmosDA.GetCustomersWithLastName(lastName);
                    
                }
                else
                {
                    await cosmosDA.GetCustomers();
                }
                jsonValue = JsonConvert.SerializeObject(cosmosDA.returnCustomerList);
                response = req.CreateResponse(HttpStatusCode.OK, jsonValue);
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                log.Info(ex.InnerException.Message);
                response = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
