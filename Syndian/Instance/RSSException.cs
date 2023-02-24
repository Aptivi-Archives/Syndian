using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Syndian.Instance
{
    internal class RSSException : Exception
    {
        public RSSException()
        { }

        public RSSException(string message)
            : base(message)
        { }

        public RSSException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
