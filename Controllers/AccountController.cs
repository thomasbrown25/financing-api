using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Account;
using financing_api.Services.AccountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace financing_api.Controllers
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

        [Authorize]
        [HttpGet("balance")] // Get all accounts
        public async Task<ActionResult<ServiceResponse<GetAccountsDto>>> GetAccountsBalance()
        {
            var response = await _accountService.GetAccountsBalance();

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("balance/{accountId}")] // Get all accounts
        public async Task<ActionResult<ServiceResponse<GetAccountsDto>>> GetAccountsBalance(string accountId)
        {
            var response = await _accountService.GetAccountBalance(accountId);

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("balance/refresh")] // Get all accounts
        public async Task<ActionResult<ServiceResponse<GetAccountsDto>>> RefreshAccountsBalance()
        {
            var response = await _accountService.RefreshAccountsBalance();

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{accountId}")]
        public async Task<ActionResult<ServiceResponse<GetAccountsDto>>> DeleteIncome(string accountId)
        {
            var response = await _accountService.DeleteAccount(accountId);

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}