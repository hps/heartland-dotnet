// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsReversal.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS reversal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities
{
    public class HpsReversal : HpsTransaction
    {
        /// <summary>Gets or sets the AVS result code.</summary>
        public string AvsResultCode { get; set; }

        /// <summary>Gets or sets the CVV result code.</summary>
        public string CvvResultCode { get; set; }

        /// <summary>Gets or sets the AVS result text.</summary>
        public string AvsResultText { get; set; }

        /// <summary>Gets or sets the CVV result text.</summary>
        public string CvvResultText { get; set; }

        /// <summary>Gets or sets the CPC indicator.</summary>
        public string CpcIndicator { get; set; }

        internal new HpsReversal FromResponse(PosResponseVer10 response) {
            var reverseResponse = (AuthRspStatusType)response.Transaction.Item;

            base.FromResponse(response);
            AvsResultCode = reverseResponse.AVSRsltCode;
            AvsResultText = reverseResponse.AVSRsltText;
            CpcIndicator = reverseResponse.CPCInd;
            CvvResultCode = reverseResponse.CVVRsltCode;
            CvvResultText = reverseResponse.CVVRsltText;

            return this;
        }
    }
}
