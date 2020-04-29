using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDA.Models
{
    public class CustomerEvent
    {
        [JsonProperty(PropertyName = "id")]
        public string CustomerId { get; set; }
        public string AgentId { get; set; }
    }
    public class CustomerSendEvent
    {
        public string CustomerId { get; set; }
        public string AgentId { get; set; }
    }
}
