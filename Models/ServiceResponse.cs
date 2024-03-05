using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string InnerException { get; set; }
        public Error Error { get; set; }
        public Going.Plaid.Errors.PlaidError PlaidError { get; set; }
    }

}