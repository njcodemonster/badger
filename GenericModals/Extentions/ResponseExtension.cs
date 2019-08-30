using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenericModals.Extentions
{
    public static class ResponseHelper
    {
        public static ResponseModel GetResponse(object data)
        {
            return new ResponseModel { Status = System.Net.HttpStatusCode.OK, Message = "", Data = data };
        }
    }
}
