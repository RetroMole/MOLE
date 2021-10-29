using System;

namespace Mole.Shared.Exceptions
{
    /// <summary>
    /// Exception, gets thrown when we get a hash mismatch
    /// </summary>
    public class HashMismatchException : Exception
    {
        public HashMismatchException(string message) : base(message) { }
    }
}