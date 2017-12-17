using System;
using System.IO;
using System.Linq;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class BatchCloseResponse : PaxDeviceResponse {
        private HostResponse hostResponse;

        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TimeStamp { get; set; }
        public string TID { get; set; }
        public string MID { get; set; }

        internal BatchCloseResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.B01_RSP_BATCH_CLOSE) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            this.hostResponse = new HostResponse(br);
            this.TotalCount = br.ReadToCode(ControlCodes.FS).Split('=').Sum(o => Convert.ToInt32(o));
            this.TotalAmount = br.ReadToCode(ControlCodes.FS).Split('=').Sum(o => Convert.ToDecimal(o) / 100);
            this.TimeStamp = br.ReadToCode(ControlCodes.FS);
            this.TID = br.ReadToCode(ControlCodes.FS);
            this.MID = br.ReadToCode(ControlCodes.ETX);

            this.HostResponse = hostResponse;
        }
    }
}
