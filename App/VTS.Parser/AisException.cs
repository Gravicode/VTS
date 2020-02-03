using System;

namespace VTS.Parser
{
    public class AisException : Exception
    {
        public AisException(string message)
            : base(message)
        {
        }
    }
}