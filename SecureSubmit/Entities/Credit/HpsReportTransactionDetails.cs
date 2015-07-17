// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsReportTransactionDetails.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS report transaction details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities {
    using SecureSubmit.Infrastructure;
    using System;
    using Hps.Exchange.PosGateway.Client;
    using SecureSubmit.Infrastructure.Validation;

    public class HpsReportTransactionDetails : HpsAuthorization, IHpsReportTransaction {
        /// <summary>Gets or sets the issuer transaction ID.</summary>
        public string IssuerTransactionId { get; set; }

        /// <summary>Gets or sets the issuer validation code.</summary>
        public string IssuerValidationCode { get; set; }

        /// <summary>Gets or sets the original transaction ID. If the transaction performed an action on a previous transaction, this field records the transaction that was acted upon.</summary>
        public int OriginalTransactionId { get; set; }

        /// <summary>Gets or sets the card number (masked).</summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>Gets or sets the settlement amount of the transaction.</summary>
        public decimal SettlementAmount { get; set; }

        /// <summary>Gets or sets the transaction type.</summary>
        public HpsTransactionType? TransactionType { get; set; }

        /// <summary>Gets or sets the date the transaction was made (UTC).</summary>
        public DateTime TransactionUtcDate { get; set; }

        /// <summary>Gets or sets a set of exceptions (if any) thrown as a result of the transaction.</summary>
        public HpsChargeExceptions Exceptions { get; set; }

        /// <summary>Gets or sets the transaction memo, a free-form field (for 
        /// Merchant reporting/record-keeping purposes only).</summary>
        public string Memo { get; set; }

        /// <summary>Gets or sets the invoice number. This will not be used at
        /// settlement. (for Merchant reporting/record-keeping purposes only).</summary>
        public string InvoiceNumber { get; set; }

        /// <summary>Gets or sets the customer ID, free-form field for 
        /// Merchant use. This is intended to be the customer identification.
        /// (for Merchant reporting/record-keeping purposes only).</summary>
        public string CustomerId { get; set; }

        internal new HpsReportTransactionDetails FromResponse(PosResponseVer10 response) {
            var reportResponse = (PosReportTxnDetailRspType)response.Transaction.Item;

            base.FromResponse(response);

            OriginalTransactionId = reportResponse.OriginalGatewayTxnId;
            TransactionType = ServiceNameToTransactionType(reportResponse.ServiceName);

            var data = reportResponse.Data;
            SettlementAmount = data.SettlementAmt;
            MaskedCardNumber = data.MaskedCardNbr;
            TransactionUtcDate = reportResponse.ReqUtcDT;
            AuthorizedAmount = data.AuthAmt;
            AvsResultCode = data.AVSRsltCode;
            AvsResultText = data.AVSRsltText;
            CardType = data.CardType;
            Descriptor = data.TxnDescriptor;
            CpcIndicator = data.CPCInd;
            CvvResultCode = data.CVVRsltCode;
            CvvResultText = data.CVVRsltText;
            ReferenceNumber = data.RefNbr;
            ResponseCode = data.RspCode;
            ResponseText = data.RspText;
            if (data.TokenizationMsg != null)
                TokenData = new HpsTokenData {
                    TokenRspMsg = data.TokenizationMsg
                };
            if (data.AdditionalTxnFields != null) {
                Memo = data.AdditionalTxnFields.Description;
                InvoiceNumber = data.AdditionalTxnFields.InvoiceNbr;
                CustomerId = data.AdditionalTxnFields.CustomerID;
            }

            if (data.RspCode != "0") {
                if (Exceptions == null)
                    Exceptions = new HpsChargeExceptions();
                Exceptions.IssuerException = HpsIssuerResponseValidation.GetException(
                    response.Header.GatewayTxnId,
                    data.RspCode,
                    data.RspText
                );
            }

            return this;
        }
    }
}