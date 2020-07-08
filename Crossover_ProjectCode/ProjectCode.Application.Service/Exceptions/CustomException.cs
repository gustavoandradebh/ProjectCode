using System;
using System.Net;

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
