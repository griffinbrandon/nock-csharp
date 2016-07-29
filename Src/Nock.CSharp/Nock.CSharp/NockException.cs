using System;

namespace NockCSharp
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