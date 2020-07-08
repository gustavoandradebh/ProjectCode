using System;
using System.Net;
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
