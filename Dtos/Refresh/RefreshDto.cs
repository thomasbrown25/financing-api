using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Account;
using financing_api.Dtos.Transaction;

namespace financing_api.Dtos.Refresh
{
    public class RefreshDto
    {
        public Task<ServiceResponse<string>> Message { get; set; }

    }
}