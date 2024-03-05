using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace financing_api.Utils
{
    public static class ControllerResponse<T>
    {
        // public static ServiceResponse<T> Validate(ServiceResponse<T> response)
        // {
        //     if (!response.Success)
        //     { // need to set this to server error
        //         return BadRequest(response);
        //     }
        //     return Ok(response);
        // }
    }
}