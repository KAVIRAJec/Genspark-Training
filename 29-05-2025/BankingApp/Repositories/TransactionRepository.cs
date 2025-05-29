using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class TransactionRepository : Repository<int, Transaction>
    {
        public TransactionRepository(BankContext context) : base(context)
        {
        }

        public override async Task<Transaction> Get(int id)
        {
            var transaction = await _context.Transactions.SingleOrDefaultAsync(t => t.Id == id);
            return transaction ?? throw new KeyNotFoundException($"Transaction with ID {id} not found.");
        }
        public override async Task<IEnumerable<Transaction>> GetAll()
        {
            var transactions = await _context.Transactions.ToListAsync();
            if (transactions == null || transactions.Count() == 0)
                throw new KeyNotFoundException("No transactions in the database.");
            return transactions;
        }
    }
}