using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TestWebApi.Models;
using TestWebApi.Utilities;

namespace TestWebApi.Controllers
{
    [CustomAuthorization]
    [LogInfo]
    public class CustomerEventController : ApiController
    {
        /// <summary>
        /// Raises and event to create a new customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetEventAsync()
        {
            HttpResponseMessage response;
            try
            {
                CustomerEventRaiser customerEventRaiser = new CustomerEventRaiser();
                string customerId = GenerateCustomerId.Generate(9);
                await customerEventRaiser.RaiseNewCustomerEvent(customerId);
                response = Request.CreateResponse(HttpStatusCode.OK, "new customer "+ customerId + " created using event hub");
            }
            catch(Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;

        }

        /// <summary>
        /// invoked by event to insert customer into db
        /// </summary>
        /// <param name="customer">customer details</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PostCustomer(CustomerEvent customer)
        {
            var url = ConfigurationManager.AppSettings["CustomerEventUrl"].ToString();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var content = JsonConvert.SerializeObject(customer);
                var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, stringContent);
                response = result.Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "Customer details entered succesfully");
                }
                return response;
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

    }
}
