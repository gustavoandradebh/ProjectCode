using System;
using System.Net;

namespace ProjectCode.Application.Service.Exceptions
{
    [Serializable]
    public class ConflictException : CustomException
    {
        public ConflictException(string message)
           : base(HttpStatusCode.Conflict, message)
        {
        }
    }
}
