using System;

namespace SecureSubmit.Infrastructure
{
    public class HpsCreditException : HpsException
    {
        public int TransactionId { get; set; }

        public HpsExceptionCodes Code { get; set; }

        public HpsCreditExceptionDetails Details { get; set; }

        public HpsCreditException(int transactionId, HpsExceptionCodes code, string message, Exception e = null)
            : base(message, e)
        {
            TransactionId = transactionId;
            Code = code;
        }

        public HpsCreditException(int transactionId, HpsExceptionCodes code, string message, string issuerCode, string issuerMessage, Exception e = null)
            : base(message, e)
        {
            TransactionId = transactionId;
            Code = code;
            Details = new HpsCreditExceptionDetails
            {
                IssuerResponseCode = issuerCode,
                IssuerResponseText = issuerMessage
            };
        }
    }
}
