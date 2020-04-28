using System;

namespace TestWebApi.Models
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public Nullable<int> Phonenumber { get; set; }
        public string TransactionID { get; set; }
        public string AgentID { get; set; }
    }
}