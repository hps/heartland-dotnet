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
using Hps.Exchange.PosGateway.Client;
    using System.Collections.Generic;
    using SecureSubmit.Infrastructure.Validation;

    /// <summary>The HPS report transaction summary.</summary>
    public class HpsReportTransactionSummary : HpsTransaction, IHpsReportTransaction
    {
        /// <summary>Gets or sets the amount of the transaction.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the settlement amount of the transaction.</summary>
        public decimal SettlementAmount { get; set; }

        /// <summary>Gets or sets the original transaction ID. If the transaction performed an action on a previous transaction, this field records the transaction that was acted upon.</summary>
        public long OriginalTransactionId { get; set; }

        /// <summary>Gets or sets the card number (masked).</summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>Gets or sets the transaction type.</summary>
        public HpsTransactionType? TransactionType { get; set; }

        /// <summary>Gets or sets the date the transaction was made (UTC).</summary>
        public DateTime TransactionUtcDate { get; set; }

        /// <summary>Gets or sets a set of exceptions (if any) thrown as a result of the transaction.</summary>
        public HpsChargeExceptions Exceptions { get; set; }

        internal HpsReportTransactionSummary[] FromResponse(PosResponseVer10 response, HpsTransactionType? filterBy = null) {
            var reportResponse = (PosReportActivityRspType)response.Transaction.Item;
            
            List<HpsReportTransactionSummary> transactions = new List<HpsReportTransactionSummary>();
            string serviceName = string.Empty;
            if (filterBy.HasValue)
                serviceName = TransactionTypeToServiceName(filterBy.Value);

            foreach (var charge in reportResponse.Details) {
                var trans = new HpsReportTransactionSummary();
                trans.FromResponse(response);

                trans.OriginalTransactionId = charge.OriginalGatewayTxnId;
                trans.MaskedCardNumber = charge.MaskedCardNbr;
                trans.ResponseCode = charge.IssuerRspCode;
                trans.ResponseText = charge.IssuerRspText;
                trans.Amount = charge.Amt;
                trans.SettlementAmount = charge.SettlementAmt;
                trans.TransactionUtcDate = charge.TxnDT;
                trans.TransactionType = ServiceNameToTransactionType(charge.ServiceName);
                if (filterBy.HasValue)
                    trans.TransactionType = filterBy.Value;

                if (charge.GatewayRspCode != 0 || charge.IssuerRspCode != "00") {
                    trans.Exceptions = new HpsChargeExceptions();
                    if (charge.GatewayRspCode != 0)
                        trans.Exceptions.GatewayException = HpsGatewayResponseValidation.GetException(charge.GatewayRspCode, charge.GatewayRspMsg);
                    if (charge.IssuerRspCode != "00")
                        trans.Exceptions.IssuerException = HpsIssuerResponseValidation.GetException(charge.GatewayRspCode, charge.IssuerRspCode, charge.IssuerRspText);
                }
                transactions.Add(trans);
            }

            return transactions.ToArray();
        }
    }
}