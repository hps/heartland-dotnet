using System;

namespace SecureSubmit.Infrastructure
{
    public class HpsGatewayException : HpsException
    {
        public HpsExceptionCodes Code { get; set; }

        public HpsGatewayExceptionDetails Details { get; set; }

        public HpsGatewayException(HpsExceptionCodes code, string sdkMessage, Exception innerException = null)
            : base(sdkMessage, innerException)
        {
            Code = code;
        }

        public HpsGatewayException(HpsExceptionCodes code, string sdkMessage, int gatewayResponseCode, string gatewayResponseMessage, Exception innerException = null)
            : base(sdkMessage, innerException)
        {
            Code = code;
            Details = new HpsGatewayExceptionDetails
            {
                GatewayResponseCode = gatewayResponseCode,
                GatewayResponseMessage = gatewayResponseMessage
            };
        }
    }
}
