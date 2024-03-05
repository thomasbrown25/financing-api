using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
        public string? Mask { get; set; }
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public decimal? BalanceAvailable { get; set; }
        public decimal? BalanceCurrent { get; set; }
        public decimal? BalanceLimit { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
