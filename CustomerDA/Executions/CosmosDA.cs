using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CustomerDA.Models;
using Microsoft.Azure.Cosmos;

namespace CustomerDA.Executions
{
    public class CosmosDA
    {
        private CosmosClient _cosmosClient;
        private string _endpointUri;
        private string _primaryKey;
        private Database _database;
        private Container _container;
        private string _databaseId;
        private string _containerId;
        public List<CustomerDetail> returnCustomerList { get; private set; }
        public CosmosDA(string endpointUri,string primaryKey,string databaseId,string containerId)
        {
            _endpointUri = endpointUri;
            _primaryKey = primaryKey;
            _cosmosClient = new CosmosClient(_endpointUri, _primaryKey);
            _databaseId = databaseId;
            _containerId = containerId;
        }

        private async Task CreateDatabaseAsync()
        {
            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
        }

        private async Task CreateContainerAsync()
        {
            _container = await _database.CreateContainerIfNotExistsAsync(_containerId, "/LastName");
        }

        private async Task AddItemsToContainerAsync(CustomerCosmos customer)
        {
            try
            {
                ItemResponse<CustomerCosmos> response = await _container.CreateItemAsync<CustomerCosmos>(customer, new PartitionKey(customer.LastName));

            }
            catch (CosmosException ex) 
            {
                throw ex;
                
            }
        }

        private async Task SelectCustomerWithLastName(string lastName)
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c WHERE (lower(c.LastName) =lower('" + lastName+"'))";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<CustomerCosmos> queryResultSetIterator = _container.GetItemQueryIterator<CustomerCosmos>(queryDefinition);

                returnCustomerList = new List<CustomerDetail>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<CustomerCosmos> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (CustomerCosmos customer in currentResultSet)
                    {
                        returnCustomerList.Add(new CustomerDetail
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Address = customer.Address,
                            Phonenumber = customer.Phonenumber
                        });
                    }
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task SelectCustomers()
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<CustomerCosmos> queryResultSetIterator = _container.GetItemQueryIterator<CustomerCosmos>(queryDefinition);

                returnCustomerList = new List<CustomerDetail>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<CustomerCosmos> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (CustomerCosmos customer in currentResultSet)
                    {
                        returnCustomerList.Add(new CustomerDetail
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Address = customer.Address,
                            Phonenumber = customer.Phonenumber
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ReadCustomerById(string id,string lastName)
        {
            try
            {
                var response = await _container.ReadItemAsync<CustomerCosmos>(id, new PartitionKey(lastName));
                var result = response.Resource;
                if (result != null)
                {
                    returnCustomerList = new List<CustomerDetail>();
                    returnCustomerList.Add(new CustomerDetail
                    {
                        FirstName = result.FirstName,
                        LastName = result.LastName,
                        Address = result.Address,
                        Phonenumber = result.Phonenumber
                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task AddCustomerItem(CustomerCosmos customer)
        {
            try
            {
                await CreateDatabaseAsync();
                await CreateContainerAsync();
                await AddItemsToContainerAsync(customer);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetCustomersWithLastName(string lastName)
        {
            try
            {
                await CreateDatabaseAsync();
                await CreateContainerAsync();
                await SelectCustomerWithLastName(lastName);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetCustomers()
        {
            try
            {
                await CreateDatabaseAsync();
                await CreateContainerAsync();
                await SelectCustomers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetCustomerbyID(string id,string lastName)
        {
            try
            {
                await CreateDatabaseAsync();
                await CreateContainerAsync();
                await ReadCustomerById(id,lastName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
