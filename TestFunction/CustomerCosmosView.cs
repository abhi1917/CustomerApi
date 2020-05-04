using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req,
        [DocumentDB(
            databaseName: "%databaseId%",
            collectionName: "%containerId%",
            ConnectionStringSetting = "AzureconnectionString", CreateIfNotExists = true, PartitionKey = "%CosmosPartitionKey%")]IEnumerable<CustomerDetail> customers, TraceWriter log)
        {
            log.Info("Customer View begins");
            string jsonValue = "";
            HttpResponseMessage response;
            try
            {
                List<CustomerDetail> returnList = null;
                if (req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value != null)
                {
                    string lastName = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", true) == 0).Value.Replace("\"", "");
                    returnList = customers.Where(s => s.LastName == lastName).ToList();

                }
                else
                {
                    returnList = customers.ToList();
                }                
                jsonValue = JsonConvert.SerializeObject(returnList);
                response = req.CreateResponse(HttpStatusCode.OK, jsonValue);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Error(ex.InnerException.Message);
                response = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
