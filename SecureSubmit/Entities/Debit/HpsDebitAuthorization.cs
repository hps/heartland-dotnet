using System;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities {
    
public class HpsDebitAuthorization : HpsTransaction {
    public string AuthorizationCode { get; set; }
    public decimal? AvailableBalance { get; set; }
    public string AvsResultCode { get; set; }
    public string CvvResultCode { get; set; }
    public string AvsResultText { get; set; }
    public string CvvResultText { get; set; }
    public string CardType { get; set; }
    public decimal? AuthorizedAmount { get; set; }

    public HpsDebitAuthorization FromResponse(PosResponseVer10 rsp) {
        base.FromResponse(rsp);

        var response = (AuthRspStatusType)rsp.Transaction.Item;
        AuthorizationCode = response.AuthCode;
        AvsResultCode = response.AVSRsltCode;
        AvsResultText = response.AVSRsltText;
        CvvResultCode = response.CVVRsltCode;
        CvvResultText = response.CVVRsltText;
        CardType = response.CardType;
        AvailableBalance = response.AvailableBalance;
        AuthorizedAmount = response.AuthAmt;

        return this;
    }
}

}
