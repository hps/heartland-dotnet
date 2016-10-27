// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsAuthorization.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS auth.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities
{
    /// <summary>The HPS auth.</summary>
    public class HpsAuthorization : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

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

        /// <summary>Gets or sets the authorized amount.</summary>
        public decimal AuthorizedAmount { get; set; }

        /// <summary>Gets or sets the card type.</summary>
        public string CardType { get; set; }

        /// <summary>Gets or sets the transaction descriptor, a description that is 
        /// concatenated to a configurable merchant DBA name. The resulting string is 
        /// sent to the card issuer for the Merchant Name.</summary>
        public string Descriptor { get; set; }

        /// <summary>Gets or sets the token data.</summary>
        public HpsTokenData TokenData { get; set; }

        public HpsGiftCardResponse RewardsResponse { get; set; }

        internal new HpsAuthorization FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);

            if (response.Transaction == null)
                return this;

            var authResponse = (AuthRspStatusType)response.Transaction.Item;

            this.AuthorizationCode = authResponse.AuthCode;
            this.AvsResultCode = authResponse.AVSRsltCode;
            this.AvsResultText = authResponse.AVSRsltText;
            this.CvvResultCode = authResponse.CVVRsltCode;
            this.CvvResultText = authResponse.CVVRsltText;
            this.AuthorizedAmount = authResponse.AuthAmt;
            this.CardType = authResponse.CardType;
            this.Descriptor = authResponse.TxnDescriptor;
            this.CpcIndicator = authResponse.CPCInd;

            if (response.Header.TokenData != null) {
                HpsTokenData token = new HpsTokenData {
                    TokenRspCode = response.Header.TokenData.TokenRspCode,
                    TokenRspMsg = response.Header.TokenData.TokenRspMsg,
                    TokenValue = response.Header.TokenData.TokenValue
                };

                this.TokenData = token;
            }

            return this;
        }
    }
}