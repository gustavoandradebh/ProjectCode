using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
namespace ProjectCode.Application.Service.Exceptions
{
    [Serializable]
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message)
           : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}
