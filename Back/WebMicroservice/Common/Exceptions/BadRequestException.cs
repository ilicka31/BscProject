using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Common.Exceptions
{

    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message, null, HttpStatusCode.BadRequest)
        {
        }
    }
}
