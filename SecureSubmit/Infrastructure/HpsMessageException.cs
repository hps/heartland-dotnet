using System;

namespace SecureSubmit.Infrastructure {
    public class HpsMessageException : HpsException {
        public HpsMessageException(string message, Exception innerException=null) : base(message, innerException) { }
    }
}
