using System;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities {
    public class HpsEbtAuthorization : HpsDebitAuthorization {
        public HpsEbtAuthorization FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);
            return this;
        }
    }
}
