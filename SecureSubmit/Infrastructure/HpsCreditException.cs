using System;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Infrastructure
{
    public class HpsCreditException : HpsException
    {
        public long TransactionId { get; set; }

        public HpsExceptionCodes Code { get; set; }

        public HpsCreditExceptionDetails Details { get; set; }

        public HpsCreditException(long transactionId, HpsExceptionCodes code, string message, Exception e = null)
            : base(message, e)
        {
            TransactionId = transactionId;
            Code = code;
        }

        public HpsCreditException(long transactionId, HpsExceptionCodes code, string message, string issuerCode, string issuerMessage, Exception e = null, AuthRspStatusType authRsp = null)
            : base(message, e)
        {
            TransactionId = transactionId;
            Code = code;
            Details = new HpsCreditExceptionDetails
            {
                IssuerResponseCode = issuerCode,
                IssuerResponseText = issuerMessage,
                EMVIssuerResp = authRsp?.EMVIssuerResp
            };
        }
    }
}
