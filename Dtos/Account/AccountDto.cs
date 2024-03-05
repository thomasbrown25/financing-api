using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Account
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccountId { get; set; }
        public string? Name { get; set; }
        public string? Mask { get; set; }
        public string? OfficialName { get; set; }
        public string? Type { get; set; }
        public string? Subtype { get; set; }
        public decimal? BalanceAvailable { get; set; }
        public decimal? BalanceCurrent { get; set; }
        public decimal? BalanceLimit { get; set; }
    }
}
