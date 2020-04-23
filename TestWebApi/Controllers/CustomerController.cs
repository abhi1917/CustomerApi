using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestWebApi.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TestWebApi.Utilities;

namespace TestWebApi.Controllers
{
    [CustomAuthorization]
    public class CustomerController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get()
        {
            var url = ConfigurationManager.AppSettings["CustomersViewUrl"].ToString();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch(Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        // POST api/values
        public HttpResponseMessage Post(Customer customer)
        {
            HttpResponseMessage response;
            HttpClient httpClient = new HttpClient();
            var url= ConfigurationManager.AppSettings["CutomerAddUrl"].ToString();
            try
            {
                if (Request.Headers.Contains("transactionID"))
                {
                    customer.TransactionID = Request.Headers.GetValues("transactionID").FirstOrDefault();
                }
                if (Request.Headers.Contains("agentID"))
                {
                    customer.AgentID = Request.Headers.GetValues("agentID").FirstOrDefault();
                }
                var content = JsonConvert.SerializeObject(customer);
                var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, stringContent);
                response = result.Result;
                return response;
            }
            catch(Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }
    }
}
