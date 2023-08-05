using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message, List<string>? errorMessages = default,
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            ErrorMessages = errorMessages;
            HttpStatusCode = httpStatusCode;
        }

        public List<string>? ErrorMessages { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }


    }
}
