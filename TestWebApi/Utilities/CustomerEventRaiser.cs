using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Configuration;

namespace TestWebApi.Utilities
{
    public class CustomerEventRaiser
    {
        private readonly string connectionString = Environment.GetEnvironmentVariable("EventHubConnectionString");
        private readonly string eventHubName = Environment.GetEnvironmentVariable("EventHubName");

        /// <summary>
        /// Raises an event to create customer
        /// </summary>
        /// <param name="customerId">customerId to be inserted</param>
        /// <returns></returns>
        public async Task RaiseNewCustomerEvent(string customerId)
        {
            EventHubProducerClient producerClient = new EventHubProducerClient(connectionString, eventHubName);
            EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
            
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(customerId)));

           
            // Use the producer client to send the batch of events to the event hub
            await producerClient.SendAsync(eventBatch);
        }
    }
}