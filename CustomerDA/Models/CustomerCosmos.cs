using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDA.Models
{
    public class CustomerCosmos
    {
        [JsonProperty(PropertyName = "id")]
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Phonenumber { get; set; }
        public string TransactionID { get; set; }
        public string AgentID { get; set; }
    }
}
