using BankingApp.Models.DTOs;

namespace BankingApp.Interfaces
{
    public interface IOtherContextFunctionalities
    {
        Task<TransactionResponseDto> TransferFunds(TransferRequestDto transferDto);
    }
}