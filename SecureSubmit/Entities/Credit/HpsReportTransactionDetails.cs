// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsReportTransactionDetails.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS report transaction details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using SecureSubmit.Infrastructure;
    using System;

    public class HpsReportTransactionDetails : HpsAuthorization, IHpsReportTransaction
    {
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
    }
}