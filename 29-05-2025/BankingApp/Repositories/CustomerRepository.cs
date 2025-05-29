using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class CustomerRepository : Repository<int, Customer>
    {
        public CustomerRepository(BankContext context) : base(context)
        {
        }
        public override async Task<Customer> Get(int id)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Id == id);
            return customer ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }
        public override async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();
            if (customers == null || customers.Count() == 0)
                throw new KeyNotFoundException("No customers in the database.");
            return customers;
        }
    }
}