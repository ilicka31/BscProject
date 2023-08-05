using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Common.Exceptions
{
    public class InternalServerException : CustomException
    {
        public InternalServerException(string message, List<string>? errorMessages = default) : base(message, errorMessages, HttpStatusCode.InternalServerError)
        {
        }
    }
}
