using System;

namespace SecureSubmit.Infrastructure {
    public class HpsArgumentException : HpsException {
        public HpsArgumentException(string message) : base(message) { }
    }
}
