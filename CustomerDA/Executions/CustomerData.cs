using CustomerDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDA.Executions
{
    public class CustomerData
    {
        WebapiDBEntities _dbContext;

        public CustomerData(string con)
        {
            _dbContext = new WebapiDBEntities(con);
        }

        public void Store(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
        }
    }
}
