// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsReportTransactionSummary.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS report transaction summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using SecureSubmit.Infrastructure;
    using System;

    /// <summary>The HPS report transaction summary.</summary>
    public class HpsReportTransactionSummary : HpsTransaction, IHpsReportTransaction
    {
        /// <summary>Gets or sets the amount of the transaction.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the settlement amount of the transaction.</summary>
        public decimal SettlementAmount { get; set; }

        /// <summary>Gets or sets the original transaction ID. If the transaction performed an action on a previous transaction, this field records the transaction that was acted upon.</summary>
        public int OriginalTransactionId { get; set; }

        /// <summary>Gets or sets the card number (masked).</summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>Gets or sets the transaction type.</summary>
        public HpsTransactionType? TransactionType { get; set; }

        /// <summary>Gets or sets the date the transaction was made (UTC).</summary>
        public DateTime TransactionUtcDate { get; set; }

        /// <summary>Gets or sets a set of exceptions (if any) thrown as a result of the transaction.</summary>
        public HpsChargeExceptions Exceptions { get; set; }
    }
}