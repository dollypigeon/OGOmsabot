using Bot_Application1.Datamodels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Customer> CustomerTable;
        private IMobileServiceTable<Rides> RidesTable;
        private AzureManager()
        {
            this.client = new MobileServiceClient("http://dentalbot.azurewebsites.net/");
            this.CustomerTable = this.client.GetTable<Customer>();
            this.RidesTable = this.client.GetTable<Rides>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }
        public async Task<List<Customer>> GetCustomers()
        {
            return await this.CustomerTable.ToListAsync();
        }
        public async Task AddCustomer(Customer customer)
        {
            await this.CustomerTable.InsertAsync(customer);
        }
        public async Task UpdateCustomer(Customer customer)
        {
            await this.CustomerTable.UpdateAsync(customer);
        }
        public async Task DeleteCustomer(Customer customer)
        {
            await this.CustomerTable.DeleteAsync(customer);
        }
        public async Task<List<Rides>> GetRides()
        {
            return await this.RidesTable.ToListAsync();
        }
    }
}