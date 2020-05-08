using System;
using System.Runtime.Serialization;

namespace DatumCollection.Exceptions
{
    internal class SpiderException : Exception
    {
        public SpiderException()
        {
        }

        public SpiderException(string message) : base(message)
        {
        }

    }
}