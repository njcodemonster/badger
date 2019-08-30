using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GenericModals
{
    public class ResponseModel
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
