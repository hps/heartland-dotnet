using System;
using System.Collections.Generic;
using System.Text;

namespace SecureSubmit.Serialization
{
    public class HpsError
    {
        public string type { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public string param { get; set; }
    }
}
