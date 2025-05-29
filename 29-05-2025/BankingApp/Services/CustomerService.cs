using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Models.DTOs;

namespace BankingApp.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<int, Customer> _customerRepository;

        public CustomerService(IRepository<int, Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Customer> CreateCustomer(CustomerCreateRequestDTO customerCreateRequest)
        {
            try
            {
                if (customerCreateRequest == null)
                    throw new ArgumentNullException(nameof(customerCreateRequest), "Customer creation data is required.");

                var customer = new Customer
                {
                    Name = customerCreateRequest.Name,
                    Email = customerCreateRequest.Email,
                    PhoneNumber = customerCreateRequest.PhoneNumber,
                    Status = "Active"
                };
                return await _customerRepository.Add(customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAll();
                if (customers == null || customers.Count() == 0)
                    throw new KeyNotFoundException("No customers found.");

                return customers.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Customer> GetCustomerDetails(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid customer ID.");

                var customer = await _customerRepository.Get(id);
                if (customer == null)
                    throw new KeyNotFoundException($"Customer with ID {id} not found.");

                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Customer> UpdateCustomer(CustomerUpdateRequestDTO customerUpdateRequest)
        {
            try
            {
                if (customerUpdateRequest == null || customerUpdateRequest.Id <= 0)
                    throw new ArgumentNullException(nameof(customerUpdateRequest), "Valid customer update data is required.");

                var customer = await _customerRepository.Get(customerUpdateRequest.Id);
                if (customer == null)
                    throw new KeyNotFoundException($"Customer with ID {customerUpdateRequest.Id} not found.");

                customer.Name = customerUpdateRequest.Name;
                customer.Email = customerUpdateRequest.Email;
                customer.PhoneNumber = customerUpdateRequest.PhoneNumber;
                customer.Status = customerUpdateRequest.Status;

                return await _customerRepository.Update(customer.Id,customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Customer> DeleteCustomer(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid customer ID.");

                var customer = await _customerRepository.Get(id);
                if (customer == null)
                    throw new KeyNotFoundException($"Customer with ID {id} not found.");

                return await _customerRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}