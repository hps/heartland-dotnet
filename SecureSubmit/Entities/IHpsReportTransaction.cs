// <copyright file="IHpsReportTransaction.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS report transaction interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using SecureSubmit.Infrastructure;
    using System;

    /// <summary>The HPS Report Charge</summary>
    public interface IHpsReportTransaction
    {
        /// <summary>Gets or sets the original transaction ID. If the transaction performed an action on a previous transaction, this field records the transaction that was acted upon.</summary>
        int OriginalTransactionId { get; set; }

        /// <summary>Gets or sets the card number (masked).</summary>
        string MaskedCardNumber { get; set; }

        /// <summary>Gets or sets the settlement amount of the transaction.</summary>
        decimal SettlementAmount { get; set; }

        /// <summary>Gets or sets the transaction type.</summary>
        HpsTransactionType? TransactionType { get; set; }

        /// <summary>Gets or sets the date the transaction was made (UTC).</summary>
        DateTime TransactionUtcDate { get; set; }

        /// <summary>Gets or sets a set of exceptions (if any) thrown as a result of the transaction.</summary>
        HpsChargeExceptions Exceptions { get; set; }
    }
}