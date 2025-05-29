using BankingApp.Models.DTOs;
using BankingApp.Models;
using System.Collections.Generic;

namespace BankingApp.Interfaces
{
    public interface IAccountService
    {
        Task<AccountBalanceDTO> GetAccountBalance(string accountId);
        Task<Account> GetAccountDetails(string accountId);
        Task<Account> CreateAccount(AccountCreateRequestDto accountCreationDto);
        Task<Account> CloseAccount(string accountId);
    }
}