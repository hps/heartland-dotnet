using System;
using System.IO;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class GiftResponse : PaxDeviceResponse {
        public string AuthorizationCode { get; set; }
        public decimal? BalanceAmount { get; set; }
        //TODO: public decimal? SplitTenderCardAmount { get; set; }
        //TODO: public decimal? SplitTenderBalanceDue { get; set; }
        //TODO: public decimal? PointsBalanceAmount { get; set; }
        //TODO: public string Rewards { get; set; }
        //TODO: public string Notes { get; set; }

        internal GiftResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T07_RSP_DO_GIFT, PAX_MSG_ID.T09_RSP_DO_LOYALTY) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseCode == "000000") {
                HostResponse = new HostResponse(br);
                TransactionType = br.ReadToCode(ControlCodes.FS);
                AmountResponse = new AmountResponse(br);
                AccountResponse = new AccountResponse(br);
                TraceResponse = new TraceResponse(br);
                ExtDataResponse = new ExtDataSubGroup(br);

                MapResponse();
            }
        }

        protected override void MapResponse() {
            base.MapResponse();

            // Host Response
            if (HostResponse != null) {
                AuthorizationCode = HostResponse.AuthCode;
            }

            if (AmountResponse != null) {
                BalanceAmount = AmountResponse.Balance1;
            }
        }
    }
}
