using System;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities.Credit {
    public class HpsAdditionalAmount {
        public amtTypeType AmountType { get; set; }
        public decimal Amount { get; set; }
    }
}
