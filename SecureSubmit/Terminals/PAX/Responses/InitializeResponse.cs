using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SecureSubmit.Terminals.Extensions;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Terminals.PAX {
    public class InitializeResponse : PaxDeviceResponse {
        public string SerialNumber { get; set; }

        internal InitializeResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.A01_RSP_INITIALIZE) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);
            this.SerialNumber = br.ReadToCode(ControlCodes.ETX);
        }
    }
}
