using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace financing_api.Models
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AccountType
    {
        [EnumMember(Value = "Checking")]
        Checking,

        [EnumMember(Value = "Saving")]
        Saving
    }
}
