using System;

namespace SecureSubmit.Terminals.PAX {
    public class BatchClearResponse : PaxDeviceResponse {
        public BatchClearResponse(byte[] buffer) : base(buffer, PAX_MSG_ID.B05_RSP_BATCH_CLEAR) { }
    }
}
