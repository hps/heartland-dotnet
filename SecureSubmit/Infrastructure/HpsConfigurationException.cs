using System;

namespace SecureSubmit.Infrastructure {
    public class HpsConfigurationException : HpsException {
        public HpsConfigurationException(string message, Exception innerException = null)
            : base(message, innerException) {
        }
    }
}
