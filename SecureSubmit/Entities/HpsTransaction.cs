// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsTransaction.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsTransaction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Infrastructure;
using System;

namespace SecureSubmit.Entities
{
    /// <summary>The HPS transaction.</summary>
    public class HpsTransaction
    {
        internal HpsTransactionHeader Header { get; set; }

        /// <summary>Gets or sets the gateway TXN ID.</summary>
        public int TransactionId { get; set; }

        /// <summary>Gets or sets the client specified TXN ID.</summary>
        public long? ClientTransactionId { get; set; }

        /// <summary>Gets or sets the RSP code.</summary>
        public string ResponseCode { get; set; }

        /// <summary>Gets or sets the RSP text.</summary>
        public string ResponseText { get; set; }

        /// <summary>Gets or sets the reference number.</summary>
        public string ReferenceNumber { get; set; }

        internal static string TransactionTypeToServiceName(HpsTransactionType transactionType)
        {
            switch (transactionType)
            {
                case HpsTransactionType.Authorize: return ItemChoiceType1.CreditAuth.ToString();
                case HpsTransactionType.Capture: return ItemChoiceType1.CreditAddToBatch.ToString();
                case HpsTransactionType.Charge: return ItemChoiceType1.CreditSale.ToString();
                case HpsTransactionType.Refund: return ItemChoiceType1.CreditReturn.ToString();
                case HpsTransactionType.Reverse: return ItemChoiceType1.CreditReversal.ToString();
                case HpsTransactionType.Verify: return ItemChoiceType1.CreditAccountVerify.ToString();
                case HpsTransactionType.List: return ItemChoiceType1.ReportActivity.ToString();
                case HpsTransactionType.Get: return ItemChoiceType1.ReportTxnDetail.ToString();
                case HpsTransactionType.Void: return ItemChoiceType1.CreditVoid.ToString();
                case HpsTransactionType.BatchClose: return ItemChoiceType1.BatchClose.ToString();
                case HpsTransactionType.SecurityError: return "SecurityError";
                default: return String.Empty;
            }
        }

        internal static HpsTransactionType? ServiceNameToTransactionType(string serviceName)
        {
            if (serviceName == ItemChoiceType1.CreditAddToBatch.ToString()) 
            {
                return HpsTransactionType.Capture;
            }

            if (serviceName == ItemChoiceType1.CreditSale.ToString())
            {
                return HpsTransactionType.Charge;
            }

            if (serviceName == ItemChoiceType1.CreditReturn.ToString())
            {
                return HpsTransactionType.Refund;
            }

            if (serviceName == ItemChoiceType1.CreditReversal.ToString())
            {
                return HpsTransactionType.Reverse;
            }

            if (serviceName == ItemChoiceType1.CreditAuth.ToString())
            {
                return HpsTransactionType.Authorize;
            }

            if (serviceName == ItemChoiceType1.CreditAccountVerify.ToString())
            {
                return HpsTransactionType.Verify;
            }

            if (serviceName == ItemChoiceType1.ReportActivity.ToString())
            {
                return HpsTransactionType.List;
            }

            if (serviceName == ItemChoiceType1.ReportTxnDetail.ToString())
            {
                return HpsTransactionType.Get;
            }

            if (serviceName == ItemChoiceType1.CreditVoid.ToString())
            {
                return HpsTransactionType.Void;
            }

            if (serviceName == ItemChoiceType1.BatchClose.ToString())
            {
                return HpsTransactionType.BatchClose;
            }

            if (serviceName == "SecurityError")
            {
                return HpsTransactionType.SecurityError;
            }

            return null;
        }

        internal HpsTransaction FromResponse(PosResponseVer10 response) {
            var header = response.Header;

            long? clientTransactionId = null;
            if (header.ClientTxnIdSpecified)
                clientTransactionId = header.ClientTxnId;

            this.Header = new HpsTransactionHeader {
                ClientTxnId = clientTransactionId,
                GatewayRspCode = header.GatewayRspCode,
                GatewayRspMsg = header.GatewayRspMsg,
                RspDt = header.RspDT
            };

            this.TransactionId = header.GatewayTxnId;
            this.ClientTransactionId = clientTransactionId;

            if (response.Transaction != null) {
                var transaction = response.Transaction.Item;

                var rspCodeField = transaction.GetType().GetProperty("RspCode");
                if (rspCodeField != null) {
                    var rspCode = rspCodeField.GetValue(transaction);
                    if (rspCode != null)
                        this.ResponseCode = rspCode.ToString();
                }

                var rspTextField = transaction.GetType().GetProperty("RspText");
                if (rspTextField != null) {
                    var rspText = rspTextField.GetValue(transaction);
                    if (rspText != null)
                        this.ResponseText = (string)rspText;
                }

                var refNbrField = transaction.GetType().GetProperty("RefNbr");
                if (refNbrField != null) {
                    var refNbr = refNbrField.GetValue(transaction);
                    if (refNbr != null)
                        this.ReferenceNumber = (string)refNbr;
                }
            }

            return this;
        }
    }
}
