using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Models.DTOs;

namespace BankingApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<string, Account> _accountRepository;

        public AccountService(IRepository<string, Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<AccountBalanceDTO> GetAccountBalance(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                    throw new ArgumentException("Account number is required.");

                var account = await _accountRepository.Get(accountId);
                if (account == null)
                    throw new KeyNotFoundException($"Account {accountId} not found.");

                return new AccountBalanceDTO
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Account> GetAccountDetails(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                    throw new ArgumentException("Account number is required.");

                var account = await _accountRepository.Get(accountId);
                if (account == null)
                    throw new KeyNotFoundException($"Account {accountId} not found.");

                return account;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Account> CreateAccount(AccountCreateRequestDto accountCreationDto)
        {
            try
            {
                if (accountCreationDto == null)
                    throw new ArgumentNullException(nameof(accountCreationDto), "Account creation data is required.");
                if (accountCreationDto.AccountType != "Saving" && accountCreationDto.AccountType != "Student" && accountCreationDto.AccountType != "Business" && accountCreationDto.AccountType != "Loan")
                    throw new ArgumentException("Invalid account type.");

                var newAccount = new Account
                {
                    CustomerId = accountCreationDto.CustomerId,
                    Balance = accountCreationDto.InitialBalance,
                    AccountType = (BAccountType)Enum.Parse(typeof(BAccountType), accountCreationDto.AccountType)
                };

                await _accountRepository.Add(newAccount);
                return newAccount;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Account> CloseAccount(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                    throw new ArgumentException("Account number is required.");

                var account = await _accountRepository.Get(accountId);
                if (account == null)
                    throw new KeyNotFoundException($"Account {accountId} not found.");

                account.Status = "Closed";
                return await _accountRepository.Update(account.AccountNumber, account);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}