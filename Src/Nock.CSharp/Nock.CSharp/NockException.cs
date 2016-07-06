using System;
using System.Net;

namespace Nock.CSharp
{
    public class NockException : Exception
    {
        public NockException(string message) : base(message)
        {            
        }

        public NockException(string message, Exception innerException) : base(message, innerException)
        {            
        }
    }
}