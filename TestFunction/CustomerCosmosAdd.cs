using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerDA.Executions;
using CustomerDA.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TestFunction
{
    public static class CustomerCosmosAdd
    {
        [FunctionName("CustomerCosmosAdd")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req,
            [DocumentDB(
            databaseName: "%databaseId%",
            collectionName: "%containerId%",
            ConnectionStringSetting = "AzureconnectionString",CreateIfNotExists = true,PartitionKey ="%CustomerPartitionKey%")]out object customer
            , TraceWriter log)
        {
            try
            {
                customer = req.Content.ReadAsAsync<object>().Result;
                if (null != customer)
                {
                    log.Info("Insertion done");
                    return req.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    throw new Exception("Failed to serialize object!");
                }
            }
            catch (Exception ex)
            {
                customer = null;
                log.Error(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
            }
        }
    }
}
