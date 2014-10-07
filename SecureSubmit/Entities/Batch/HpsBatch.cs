// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsChargeTransaction.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS batch transaction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    /// <summary>The HPS batch</summary>
    public class HpsBatch
    {
        /// <summary>Gets or sets the batch ID.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the transaction count.</summary>
        public int TransactionCount { get; set; }

        /// <summary>Gets or sets the total amount.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Gets or sets the sequence number.</summary>
        public int SequenceNumber { get; set; }
    }
}
