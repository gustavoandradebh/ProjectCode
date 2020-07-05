using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace ProjectCode.Application.Service.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException(HttpStatusCode code, string message)
            : base(message)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; private set; }
    }
}
