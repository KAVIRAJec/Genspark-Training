using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Models.DTOs;
using BankingApp.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<int,Transaction> _transactionRepository;
        private readonly IRepository<string, Account> _accountRepository;
        private readonly BankContext _context;
        private readonly IOtherContextFunctionalities _otherContextFunctionalities;

        public TransactionService(IRepository<int, Transaction> transactionRepository,
                                  IRepository<string, Account> accountRepository,
                                  BankContext context,
                                  IOtherContextFunctionalities otherContextFunctionalities)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _context = context;
            _otherContextFunctionalities = otherContextFunctionalities;
        }

        public async Task<TransactionResponseDto> TransferFunds(TransferRequestDto transferDto)
        {
            try
            {
                if (transferDto == null)
                    throw new ArgumentNullException(nameof(transferDto), "Transfer request cannot be null");

                var result = await _otherContextFunctionalities.TransferFunds(transferDto);
                return result;
            } 
            catch (Exception ex)
            {
                throw new ApplicationException($"Transfer failed: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionHistory(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                    throw new ArgumentException("Account number is required.");

                var transactions = await _transactionRepository.GetAll();
                var result = transactions.Where(t => t.AccountNumber == accountId).OrderBy(t => t.TransactionDate).ToList();

                if (!result.Any())
                    throw new KeyNotFoundException($"No transactions found for account {accountId}.");
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<bool> Deposit(DepositRequestDto depositDto)
        {
            if (depositDto == null || string.IsNullOrEmpty(depositDto.AccountNumber) || depositDto.Amount <= 0)
                throw new ArgumentException("Invalid deposit request.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == depositDto.AccountNumber && a.Status == "Active");
                if (account == null)
                    throw new KeyNotFoundException($"Account {depositDto.AccountNumber} not found or inactive.");

                account.Balance += depositDto.Amount;
                _context.Accounts.Update(account);

                var txn = new Transaction
                {
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Deposit",
                    Amount = depositDto.Amount,
                    TransactionDate = DateTime.UtcNow
                };
                _context.Transactions.Add(txn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> Withdraw(WithdrawalRequestDto withdrawDto)
        {
            if (withdrawDto == null || string.IsNullOrEmpty(withdrawDto.AccountNumber) || withdrawDto.Amount <= 0)
                throw new ArgumentException("Invalid withdrawal request.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == withdrawDto.AccountNumber && a.Status == "Active");
                if (account == null)
                    throw new KeyNotFoundException($"Account {withdrawDto.AccountNumber} not found or inactive.");

                if (account.Balance < withdrawDto.Amount)
                    throw new InvalidOperationException("Insufficient funds.");

                account.Balance -= withdrawDto.Amount;
                _context.Accounts.Update(account);

                var txn = new Transaction
                {
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Withdrawal",
                    Amount = withdrawDto.Amount,
                    TransactionDate = DateTime.UtcNow
                };
                _context.Transactions.Add(txn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}