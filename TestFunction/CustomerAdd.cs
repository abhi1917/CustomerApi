using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerDA.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TestFunction
{
    public static class CustomerAdd
    {
        [FunctionName("CustomerAdd")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Customer Insertion begins");

            HttpResponseMessage response;
            try
            {
                var customer = await req.Content.ReadAsAsync<Customer>();
                if (customer != null)
                {
                    string connectionString= System.Configuration.ConfigurationManager.ConnectionStrings["WebapiDBEntities"].ConnectionString;
                    using (var db = new WebapiDBEntities(connectionString))
                    {
                        db.Customers.Add(customer);
                        await db.SaveChangesAsync();
                    }
                    response = req.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                response = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
