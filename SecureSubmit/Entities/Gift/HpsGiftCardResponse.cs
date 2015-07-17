using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities {
    public class HpsGiftCardResponse : HpsTransaction {
        public string AuthorizationCode { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? PointsBalanceAmount { get; set; }
        public string Rewards { get; set; }
        public string Notes { get; set; }

        internal new HpsGiftCardResponse FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);
            TransactionId = response.Header.GatewayTxnId;

            string[] propertyNames = { "AuthCode", "BalanceAmt", "PointsBalanceAmt", "Rewards", "Notes", "RspCode", "RspText" };
            string[] valuesNames = { "AuthorizationCode", "BalanceAmount", "PointsBalanceAmount", "Rewards", "Notes", "ResponseCode", "ResponseText" };
            var transaction = response.Transaction.Item;
            for (int i = 0; i < propertyNames.Length; i++) {
                var propertyInfo = transaction.GetType().GetProperty(propertyNames[i]);
                if (propertyInfo != null) {
                    var value = propertyInfo.GetValue(transaction);

                    var valueInfo = this.GetType().GetProperty(valuesNames[i]);
                    if (valueInfo != null) {
                        try {
                            valueInfo.SetValue(this, value);
                        }
                        catch {
                            valueInfo.SetValue(this, value.ToString());
                        }
                    }
                }
            }

            return this;
        }
    }
}
