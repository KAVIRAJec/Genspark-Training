using BankingApp.Models;
using BankingApp.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingApp.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomer(CustomerCreateRequestDTO customerCreateRequest);
        Task<Customer> GetCustomerDetails(int id);
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer> UpdateCustomer(CustomerUpdateRequestDTO customerUpdateRequest);
        Task<Customer> DeleteCustomer(int id);
    }
}