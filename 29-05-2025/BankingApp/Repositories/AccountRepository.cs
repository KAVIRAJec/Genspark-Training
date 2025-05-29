using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class AccountRepository : Repository<string, Account>
    {
        public AccountRepository(BankContext context) : base(context)
        {
        }

        public override async Task<Account> Get(string accountNumber)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);
            return account ?? throw new KeyNotFoundException($"Account with number {accountNumber} not found.");
        }

        public override async Task<IEnumerable<Account>> GetAll()
        {
            var accounts = await _context.Accounts.ToListAsync();
            if (accounts == null || accounts.Count() == 0)
                throw new KeyNotFoundException("No accounts in the database.");
            return accounts;
        }
    }
}