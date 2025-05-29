using BankingApp.Models.DTOs;
using BankingApp.Contexts;
using BankingApp.Interfaces;


namespace BankingApp.Misc
{
    public class OtherContextFunctionalities : IOtherContextFunctionalities
    {
        private readonly BankContext _bankingContext;

        public OtherContextFunctionalities(BankContext bankingContext)
        {
            _bankingContext = bankingContext;
        }

        public async Task<TransactionResponseDto> TransferFunds(TransferRequestDto transferDto)
        {
            try
            {
                if (transferDto == null)
                    throw new ArgumentNullException(nameof(transferDto), "Transfer request cannot be null");
                if (string.IsNullOrEmpty(transferDto.AccountNumber) || string.IsNullOrEmpty(transferDto.PayeeAccountNumber))
                    throw new ArgumentException("Account numbers cannot be null or empty");
                if (transferDto.Amount <= 0)
                    throw new ArgumentException("Amount must be greater than zero");

                var result = await _bankingContext.TransferFunds(transferDto);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}