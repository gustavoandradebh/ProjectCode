using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace ProjectCode.Application.Service.Exceptions
{
    [Serializable]
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}
