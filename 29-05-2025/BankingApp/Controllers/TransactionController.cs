using BankingApp.Interfaces;
using BankingApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequestDto dto)
        {
            try
            {
                var result = await _transactionService.Deposit(dto);
                return result ? Ok("Deposit successful.") : BadRequest("Deposit failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawalRequestDto dto)
        {
            try
            {
                var result = await _transactionService.Withdraw(dto);
                return result ? Ok("Withdrawal successful.") : BadRequest("Withdrawal failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            try
            {
                var result = await _transactionService.TransferFunds(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetTransactionHistory(string accountId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionHistory(accountId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}