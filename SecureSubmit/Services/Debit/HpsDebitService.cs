using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services.Fluent.Debit;

namespace SecureSubmit.Services.Debit
{
    public class HpsDebitService : HpsSoapGatewayService
    {
        public HpsDebitService(IHpsServicesConfig config = null)
            : base(config)
        {
        }

        public ChargeBuilder Charge(decimal amount, string trackData, string pinBlock)
        {
            return new ChargeBuilder(ServicesConfig, amount, trackData, pinBlock);
        }

        public AddValueBuilder AddValue(decimal amount, string trackData, string pinBlock)
        {
            return new AddValueBuilder(ServicesConfig, amount, trackData, pinBlock);
        }

        public ReturnBuilder Return(decimal amount, string trackData, string pinBlock)
        {
            return new ReturnBuilder(ServicesConfig, amount, trackData, pinBlock);
        }

        public ReverseBuilder.ReverseUsingBuilder Reverse(decimal amount)
        {
            HpsInputValidation.CheckAmount(amount);
            return new ReverseBuilder.ReverseUsingBuilder(new ReverseBuilder(ServicesConfig, amount));
        }

        /// <summary>
        /// The Debit Add Value transaction adds value to a stored value card. The transaction is placed in the current
        /// open batch. If a batch is not open, this transaction creates an open batch.
        /// </summary>
        /// <param name="amount">Authorization amount.</param>
        /// <param name="currency">Currency ("usd")</param>
        /// <param name="trackData">Track data read from the card by the card reader.</param>
        /// <param name="pinBlock">PIN block.</param>
        /// <param name="allowDuplicates">Indicates whether to allow duplicate transactions.</param>
        /// <param name="cardHolder">Card holder information.</param>
        /// <param name="encryptionData">E3 encryption data group.</param>
        /// <param name="details">Group containing additional transaction fields to be included in detail reporting.</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <returns>The AddValue (Authorization) response.</returns>
        public HpsAuthorization AddValue(decimal amount, string currency, string trackData, string pinBlock,
            HpsEncryptionData encryptionData = null, bool allowDuplicates = false, HpsCardHolder cardHolder = null,
            HpsTransactionDetails details = null, long? clientTransactionId = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosDebitAddValueReqType
                {
                    Block1 = new DebitAddValueReqBlock1Type
                    {
                        TrackData = trackData,
                        AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        Amt = amount,
                        PinBlock = pinBlock,
                        EncryptionData = HydrateEncryptionData(encryptionData),
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.DebitAddValue
            };

            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitAddValue);

            var addValueRsp = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, addValueRsp.RspCode, addValueRsp.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }

        /// <summary>
        /// A Debit Return transaction returns funds to the cardholder. The transaction is generally used as a
        /// counterpart to a Debit Charge transaction that needs to be reversed. The Debit Return transaction is
        /// placed in the current open batch. If a batch is not open, this transaction create an open batch.
        /// </summary>
        /// <param name="transactionId">The gateway transaciton ID of the charge to be returned.</param>
        /// <param name="trackData">Track data read from the card by the card reader.</param>
        /// <param name="amount">Authorization amount.</param>
        /// <param name="pinBlock">PIN block.</param>
        /// <param name="allowDuplicates">Indicates whether to allow duplicate transacitons.</param>
        /// <param name="cardHolder">Card holder information.</param>
        /// <param name="encryptionData">E3 encryption data group.</param>
        /// <param name="details">Group containing additional transaction fields to be included in detail reporting.</param>
        /// <param name="clientTransactionId">Client transaction ID.</param>
        /// <returns>The Return (Authorization) results.</returns>
        public HpsAuthorization Return(int transactionId, decimal amount, string trackData, string pinBlock,
            bool allowDuplicates = false, HpsCardHolder cardHolder = null, HpsEncryptionData encryptionData = null,
            HpsTransactionDetails details = null, long? clientTransactionId = null)
        {
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosDebitReturnReqType
                {
                    Block1 = new DebitReturnReqBlock1Type
                    {
                        TrackData = trackData,
                        AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        Amt = amount,
                        PinBlock = pinBlock,
                        EncryptionData = HydrateEncryptionData(encryptionData),
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.DebitReturn
            };

            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitReturn);

            var returnRsp = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, returnRsp.RspCode, returnRsp.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }

        /// <summary>
        /// A Debit Reversal transaction reverses a Debit Charge or Debit Return transaction.
        /// </summary>
        /// <param name="transactionId">The gateway transaciton ID of the charge to be reversed.</param>
        /// <param name="trackData">The data read from the card by the card reader.</param>
        /// <param name="amount">Authorization amount.</param>
        /// <param name="authorizedAmount">Settlement amount or New Authorization amount after reversal occures.</param>
        /// <param name="encryptionData">E3 encryption data group.</param>
        /// <param name="details">Group containing additional transaction fields to be included in detail reporting.</param>
        /// <param name="clientTransactionId">The client transaction ID.</param>
        /// <returns>The reversal result.</returns>
        public HpsTransaction Reverse(int transactionId, decimal amount, string trackData, decimal? authorizedAmount = null,
            HpsEncryptionData encryptionData = null, HpsTransactionDetails details = null, long? clientTransactionId = null)
        {
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosDebitReversalReqType
                {
                    Block1 = new DebitReversalReqBlock1Type
                    {
                        GatewayTxnId = transactionId,
                        GatewayTxnIdSpecified = true,
                        TrackData = trackData,
                        Amt = amount,
                        EncryptionData = HydrateEncryptionData(encryptionData),
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.DebitReversal
            };

            if (authorizedAmount.HasValue)
            {
                var block = ((PosDebitReversalReqType) transaction.Item).Block1;
                block.AuthAmt = authorizedAmount.Value;
                block.AuthAmtSpecified = true;
            }

            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitReversal);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        /// <summary>
        /// A Debit Charge transaction performs a sale purchased with a Debit Card. The Debit Charge is placed
        /// in the current open batch. If a batch is not open, this transaction creates an open batch.
        /// </summary>
        /// <param name="amount">Authorization amount.</param>
        /// <param name="currency">Currency type ("usd").</param>
        /// <param name="trackData">Track data read from the card by the card reader.</param>
        /// <param name="pinBlock">PIN block.</param>
        /// <param name="cashBackAmount">Contains the portion of the amount that is cash back.</param>
        /// <param name="allowDuplicates">Indicates whether to allow duplicate transactions.</param>
        /// <param name="allowPartialAuth">Indicate whether to allow partial authorization.</param>
        /// <param name="cardHolder">Card holder information.</param>
        /// <param name="encryptionData">E3 encryption data group.</param>
        /// <param name="details">Group containing additional transaction fields to be inclided in detail reporting.</param>
        /// <param name="clientTransactionId">The client transaction ID.</param>
        /// <returns>The Debit Charge result.</returns>
        public HpsAuthorization Charge(decimal amount, string currency, string trackData, string pinBlock,
            HpsEncryptionData encryptionData = null, bool allowDuplicates = false, decimal? cashBackAmount = null,
            bool allowPartialAuth = false, HpsCardHolder cardHolder = null, HpsTransactionDetails details = null,
            long? clientTransactionId = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosDebitSaleReqType
                {
                    Block1 = new DebitSaleReqBlock1Type
                    {
                        TrackData = trackData,
                        AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        Amt = amount,
                        CashbackAmtInfo = cashBackAmount.HasValue ? cashBackAmount.Value : 0,
                        CashbackAmtInfoSpecified = cashBackAmount.HasValue,
                        AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        PinBlock = pinBlock,
                        EncryptionData = HydrateEncryptionData(encryptionData),
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.DebitSale
            };

            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitSale);

            var chargeResponse = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, chargeResponse.RspCode, chargeResponse.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }
    }
}
