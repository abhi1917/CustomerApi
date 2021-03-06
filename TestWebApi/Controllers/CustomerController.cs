﻿using System;
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
    [LogInfo]
    public class CustomerController : ApiController
    {
        /// <summary>
        /// /GetCustomers:This api returns a list containing all the customers with the lastName in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomers(string lastName)
        {
            var url = Environment.GetEnvironmentVariable("CustomersViewUrl")+"&lastName="+lastName;
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

        /// <summary>
        /// /GetCustomers:This api returns a list containing the customer with the id in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomersById(string id)
        {
            var url = Environment.GetEnvironmentVariable("CustomersViewByIdUrl");
            url = url.Replace("{id}", id).Replace("\"","");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        /// <summary>
        /// /GetCustomers:This api returns a list containing all the customers in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomers()
        {
            var url = Environment.GetEnvironmentVariable("CustomersViewUrl");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        /// <summary>
        /// This api posts customer data to databse
        /// </summary>
        /// <param name="customer">customer data</param>
        /// <returns>http response message</returns>
        [HttpPost]
        [CustomerDataValidation]
        public HttpResponseMessage PostCustomer(Customer customer)
        {
            HttpResponseMessage response;
            HttpClient httpClient = new HttpClient();
            var url= Environment.GetEnvironmentVariable("CutomerAddUrl").ToString();
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
                customer.id = GenerateCustomerId.Generate(9);
                var content = JsonConvert.SerializeObject(customer);
                var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, stringContent);
                response = result.Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient eventHttpClient = new HttpClient();
                    var eventurl= Environment.GetEnvironmentVariable("EventApiUrl")+"?customerId="+customer.id;
                    eventHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("authToken"));
                    var eventresponse=eventHttpClient.GetAsync(eventurl);
                    eventresponse.Wait();
                    response = Request.CreateResponse(HttpStatusCode.OK, "Customer details entered succesfully");
                }
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
