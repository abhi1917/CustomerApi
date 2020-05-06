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
    public static class CustomerCosmosViewById
    {
        [FunctionName("CustomerCosmosViewById")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "CustomerCosmosViewById/{id}")]HttpRequestMessage req,
        [DocumentDB(
            databaseName: "%databaseId%",
            collectionName: "%containerId%",
            ConnectionStringSetting = "AzureconnectionString", 
            SqlQuery ="SELECT * FROM c WHERE c.id={id}")]IEnumerable<CustomerDetail> customers, TraceWriter log)
        {
            log.Info("Customer View begins");
            string jsonValue = "";
            HttpResponseMessage response;
            try
            {
                List<CustomerDetail> returnList = customers.ToList();
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
