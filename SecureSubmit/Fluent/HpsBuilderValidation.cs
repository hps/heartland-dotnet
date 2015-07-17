using System;

namespace SecureSubmit.Fluent
{
    internal class HpsBuilderValidation
    {
        public string ExceptionMessage { get; set; }
        public Func<bool> Callback { get; set; }
    }
}
