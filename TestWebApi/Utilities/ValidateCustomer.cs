using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestWebApi.Models;

namespace TestWebApi.Utilities
{
    public static class ValidateCustomer
    {
        public static bool Validate(Customer customer)
        {
            if (customer.FirstName == null || customer.LastName == null || customer.Address == null || customer.Phonenumber == null || customer.TransactionID == null || customer.AgentID == null)
            {
                return false;
            }
            return true;
        }
    }
}