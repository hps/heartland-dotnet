using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SecureSubmit.Terminals.Extensions;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Terminals.PAX {
    public class InputAccountResponse : PaxDeviceResponse {
        public EntryMode EntryMode { get; private set; }
        public string Track1Data { get; private set; }
        public string Track2Data { get; private set; }
        public string Track3Data { get; private set; }
        public string PAN { get; private set; }
        public string ExpiryDate { get; private set; }
        public string QrCode { get; private set; }
        public string KSN { get; private set; }
        public string AdditionalInfo { get; private set; }

        internal InputAccountResponse(byte[] response) : base(response, PAX_MSG_ID.A31_RSP_INPUT_ACCOUNT) { }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseText == "OK") {
                var entryMode = (EntryMode)int.Parse(br.ReadToCode(ControlCodes.FS));
                EntryMode = entryMode;
                Track1Data = br.ReadToCode(ControlCodes.FS);
                Track2Data = br.ReadToCode(ControlCodes.FS);
                Track3Data = br.ReadToCode(ControlCodes.FS);
                PAN = br.ReadToCode(ControlCodes.FS);
                ExpiryDate = br.ReadToCode(ControlCodes.FS);
                QrCode = br.ReadToCode(ControlCodes.FS);
                KSN = br.ReadToCode(ControlCodes.FS);
                AdditionalInfo = br.ReadToCode(ControlCodes.ETX);
            }
        }
    }
}
