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
    public static class CustomersView
    {
        [FunctionName("CustomersView")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Customer View begins");

            List<CustomerDetail> customers = null;
            string jsonValue = "";
            HttpResponseMessage response;
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["WebapiDBEntities"].ConnectionString;
                using (var db = new WebapiDBEntities(connectionString))
                {
                    customers = db.Customers.Select(s => new CustomerDetail
                    {
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Address = s.Address,
                        Phonenumber = s.Phonenumber
                    }).ToList();
                    jsonValue = JsonConvert.SerializeObject(customers);
                }
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
