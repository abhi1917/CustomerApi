using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerDA.Executions;
using CustomerDA.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TestFunction
{
    public static class CustomerCosmosAdd
    {
        [FunctionName("CustomerCosmosAdd")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Customer Insertion begins");

            HttpResponseMessage response;
            try
            {
                string endpointUri = System.Environment.GetEnvironmentVariable("endpointUri");
                string primaryKey = System.Environment.GetEnvironmentVariable("customerPrimaryKey");
                string databaseId = System.Environment.GetEnvironmentVariable("databaseId");
                string containerId = System.Environment.GetEnvironmentVariable("containerId");
                CosmosDA cosmosDA = new CosmosDA(endpointUri, primaryKey, databaseId, containerId);
                var customer = await req.Content.ReadAsAsync<CustomerCosmos>();
                if (null != customer)
                {
                    await cosmosDA.AddCustomerItem(customer);
                    response = req.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    throw new Exception("Failed to serialize object!");
                }
            }
            catch(CosmosException ex)
            {
                log.Info(ex.Message);
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    response = req.CreateErrorResponse(HttpStatusCode.Conflict, "Conflict occured! Please check if customer with the name already exists in DB!");
                }
                else
                {
                    response = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
