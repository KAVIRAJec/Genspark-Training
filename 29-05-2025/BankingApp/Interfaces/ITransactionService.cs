using BankingApp.Models.DTOs;
using BankingApp.Models;
using System.Collections.Generic;

namespace BankingApp.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> TransferFunds(TransferRequestDto transferDto);
        Task<IEnumerable<Transaction>> GetTransactionHistory(string accountId);
        Task<bool> Deposit(DepositRequestDto depositDto);
        Task<bool> Withdraw(WithdrawalRequestDto withdrawDto);
    }
}