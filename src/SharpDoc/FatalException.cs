using System;

namespace SharpDoc
{
    /// <summary>
    /// FatalException used internally to exit the process
    /// </summary>
    internal class FatalException : Exception
    {
        public FatalException()
        {
        }

        public FatalException(string message) : base(message)
        {
        }
    }
}