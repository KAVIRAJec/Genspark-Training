using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateRequestDto dto)
        {
            try
            {
                var account = await _accountService.CreateAccount(dto);
                return CreatedAtAction(nameof(GetAccountDetails), new { accountId = account.AccountNumber }, account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountDetails(string accountId)
        {
            try
            {
                var account = await _accountService.GetAccountDetails(accountId);
                return Ok(account);
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

        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetAccountBalance(string accountId)
        {
            try
            {
                var balance = await _accountService.GetAccountBalance(accountId);
                return Ok(balance);
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
        [HttpDelete("{accountId}")]
        public async Task<IActionResult> CloseAccount(string accountId)
        {
            try
            {
                var account = await _accountService.CloseAccount(accountId);
                return Ok(account);
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