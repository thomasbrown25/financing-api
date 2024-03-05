using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Transaction;
using financing_api.Services.TransactionsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace financing_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(ITransactionsService transactionService)
        {
            _transactionsService = transactionService;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<ServiceResponse<string>>> GetTransactions()
        {
            var response = await _transactionsService.GetTransactions();

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("recurring")]
        public async Task<ActionResult<ServiceResponse<GetRecurringDto>>> GetRecurringTransactions()
        {
            var response = await _transactionsService.GetRecurringTransactions();

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("recent-bills")]
        public async Task<ActionResult<ServiceResponse<GetRecurringDto>>> GetRecentBills()
        {
            var response = await _transactionsService.GetRecentBills();

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<ServiceResponse<GetRecurringDto>>> GetAccountTransactions(string accountId)
        {
            var response = await _transactionsService.GetAccountTransactions(accountId);

            if (!response.Success)
            { // need to set this to server error
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
