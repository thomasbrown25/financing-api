using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Shared
{
    public class PlaidCredentials
    {
        public string? LinkToken { get; set; }
        public string? AccessToken { get; set; }
        public string? ItemId { get; set; }
        public string? Products { get; set; }
        public string? CountryCodes { get; set; }
    }
}