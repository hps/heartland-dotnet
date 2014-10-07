// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsTransactionDetails.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The transaction details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsTransactionDetails
    {
        /// <summary>Gets or sets the client-generated transaction ID. If used, the value
        /// must be unique per transaction. Returned in the response header if sent in the
        /// request.</summary>
        public long? ClientTransactionId { get; set; }

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
