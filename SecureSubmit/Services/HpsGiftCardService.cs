using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace SecureSubmit.Services
{
    public class HpsGiftCardService : HpsSoapGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsCreditService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        public HpsGiftCardService(IHpsServicesConfig config = null)
            : base(config)
        {
        }

        [Obsolete("This 'Activate' method is deprecated, please use the fluent version instead.")]
        public HpsGiftCardResponse Activate(decimal amount, string currency, HpsGiftCard giftCard)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardActivateReqType
                {
                    Block1 = new GiftCardActivateReqBlock1Type
                    {
                        Amt = amount,
                        CardData = new GiftCardDataType
                        {
                            Item = giftCard.Value,
                            ItemElementName = giftCard.ValueType,
                            EncryptionData = (giftCard.EncryptionData == null) ? null : new EncryptionDataType
                            {
                                EncryptedTrackNumber = giftCard.EncryptionData.EncryptedTrackNumber,
                                KSN = giftCard.EncryptionData.Ksn,
                                KTB = giftCard.EncryptionData.Ktb,
                                Version = giftCard.EncryptionData.Version
                            }
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardActivate
            };

            /* Submit the transaction. */
            return SubmitActivation(transaction);
        }

        /// <summary>
        /// An <b>AddValue</b> transaction is used to add an amount to the value of an active gift card.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">Transaction currency.</param>
        /// <param name="giftCard">The gift card information.</param>
        /// <returns>The <see cref="HpsGiftCardAddValue"/>.</returns>
        public HpsGiftCardResponse AddValue(decimal amount, string currency, HpsGiftCard giftCard)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardAddValueReqType
                {
                    Block1 = new GiftCardAddValueReqBlock1Type
                    {
                        Amt = amount,
                        CardData = new GiftCardDataType
                        {
                            Item = giftCard.Value,
                            ItemElementName = giftCard.ValueType,
                            EncryptionData = (giftCard.EncryptionData == null) ? null : new EncryptionDataType
                            {
                                EncryptedTrackNumber = giftCard.EncryptionData.EncryptedTrackNumber,
                                KSN = giftCard.EncryptionData.Ksn,
                                KTB = giftCard.EncryptionData.Ktb,
                                Version = giftCard.EncryptionData.Version
                            }
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardAddValue
            };

            /* Submit the transaction. */
            return SubmitAddValue(transaction);
        }

        /// <summary>
        /// A <b>Alias</b> transaction is used to manage an account number alias, such as a phone number,
        /// for a stored value account. The transaction can be used to Add an alias to an existing account,
        /// Delete an alias from an existing account or Create a new stored value account and add an alias
        /// to the new account.
        /// </summary>
        /// <param name="action">Type of Alias action requested.</param>
        /// <param name="giftCard">The gift card information (leave null for CREATE action).</param>
        /// <param name="alias">Alternate identifier used to reference a stored value account.</param>
        /// <returns>The <see cref="HpsGiftCardAlias"/>.</returns>
        public HpsGiftCardAlias Alias(HpsGiftCardAliasAction action, HpsGiftCard giftCard, string alias)
        {
            var gatewayAction = GiftCardAliasReqBlock1TypeAction.ADD;
            switch (action)
            {
                case HpsGiftCardAliasAction.Create: gatewayAction = GiftCardAliasReqBlock1TypeAction.CREATE; break;
                case HpsGiftCardAliasAction.Delete: gatewayAction = GiftCardAliasReqBlock1TypeAction.DELETE; break;
            }

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardAliasReqType
                {
                    Block1 = new GiftCardAliasReqBlock1Type
                    {
                        Action = gatewayAction,
                        Alias = alias,
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardAlias
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardAlias);

            var aliasRsp = (PosGiftCardAliasRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                aliasRsp.RspCode.ToString(CultureInfo.InvariantCulture), aliasRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardAlias
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                GiftCard = new HpsGiftCard
                {
                    Value = aliasRsp.CardData.CardNbr
                },
                ResponseCode = aliasRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = aliasRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Balance</b> transaction is used to check the balance of a gift card.
        /// </summary>
        /// <param name="giftCard">The gift card information.</param>
        /// <returns>The <see cref="HpsGiftCardBalance"/>.</returns>
        public HpsGiftCardResponse GetBalance(HpsGiftCard giftCard)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardBalanceReqType
                {
                    Block1 = new GiftCardBalanceReqBlock1Type
                    {
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardBalance
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardBalance);

            var balanceRsp = (PosGiftCardBalanceRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                balanceRsp.RspCode.ToString(CultureInfo.InvariantCulture), balanceRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = balanceRsp.AuthCode,
                BalanceAmount = balanceRsp.BalanceAmt,
                PointsBalanceAmount = balanceRsp.PointsBalanceAmt,
                Rewards = balanceRsp.Rewards,
                Notes = balanceRsp.Notes,
                ResponseCode = balanceRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = balanceRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Deactivate</b> transaction is used to permanently deactivate a gift card. Once deactivated
        /// a gift card can no longer be used for any transactions, nor can it be re-activated.
        /// </summary>
        /// <param name="giftCard">The gift card information.</param>
        /// <returns>The <see cref="HpsGiftCardBalance"/>.</returns>
        public HpsGiftCardResponse DeactivateCard(HpsGiftCard giftCard)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardDeactivateReqType
                {
                    Block1 = new GiftCardDeactivateReqBlock1Type
                    {
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardDeactivate
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardDeactivate);

            var deactivateRsp = (PosGiftCardDeactivateRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                deactivateRsp.RspCode.ToString(CultureInfo.InvariantCulture), deactivateRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = deactivateRsp.AuthCode,
                BalanceAmount = deactivateRsp.RefundAmt,
                PointsBalanceAmount = deactivateRsp.PointsBalanceAmt,
                ResponseCode = deactivateRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = deactivateRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Replace</b> transaction is used to replace an active gift card with another gift card.
        /// Once replaced, the old gift card is permanently deactivated. The new gift card is assigned
        /// the current amount of the old gift card.
        /// </summary>
        /// <param name="oldGiftCard">The old gift card information.</param>
        /// <param name="newGiftCard">The new gift card information.</param>
        /// <returns>The <see cref="HpsGiftCardReplace"/>.</returns>
        public HpsGiftCardResponse ReplaceCard(HpsGiftCard oldGiftCard, HpsGiftCard newGiftCard)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardReplaceReqType
                {
                    Block1 = new GiftCardReplaceReqBlock1Type
                    {
                        OldCardData = HydrateGiftCardData(oldGiftCard),
                        NewCardData = HydrateGiftCardData(newGiftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardReplace
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReplace);

            var replaceRsp = (PosGiftCardReplaceRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                replaceRsp.RspCode.ToString(CultureInfo.InvariantCulture), replaceRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = replaceRsp.AuthCode,
                BalanceAmount = replaceRsp.BalanceAmt,
                PointsBalanceAmount = replaceRsp.PointsBalanceAmt,
                ResponseCode = replaceRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = replaceRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Reward</b> transaction is used to add rewards to a stored value account when a
        /// purchase is made with a credit card, debit card, cash or other form of payment. The
        /// Reward transaction amount is not deduced from or loaded to the stored value account,
        /// but is used to determine the potential rewards that may be added to the account
        /// based on the merchant’s loyalty and rewards program.
        /// </summary>
        /// <param name="giftCard">The gift card information.</param>
        /// <param name="amount">The amount of purchase to be used in rewards calculation.</param>
        /// <param name="currency">Identifies the currency of a financial transaction ("usd" or "points")</param>
        /// <param name="gratuity">The portion of the purchase amount that is a gratuity</param>
        /// <param name="tax">The portion of the purchase amount that is tax</param>
        /// <returns>The <see cref="HpsGiftCardReward"/>.</returns>
        public HpsGiftCardResponse Reward(HpsGiftCard giftCard, decimal amount, string currency = "usd",
            decimal? gratuity = null, decimal? tax = null)
        {
            currency = currency.ToLower();
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardRewardReqType
                {
                    Block1 = new GiftCardRewardReqBlock1Type
                    {
                        Amt = amount,
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardReward
            };

            var block = ((PosGiftCardRewardReqType)transaction.Item).Block1;
            if (currency == "usd" || currency == "points")
            {
                block.Currency = currency == "usd" ? currencyType.USD : currencyType.POINTS;
                block.CurrencySpecified = true;
            }

            if (gratuity != null)
            {
                block.GratuityAmtInfo = gratuity.Value;
                block.GratuityAmtInfoSpecified = true;
            }

            if (tax != null)
            {
                block.TaxAmtInfo = tax.Value;
                block.TaxAmtInfoSpecified = true;
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReward);

            var rewardRsp = (PosGiftCardRewardRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                rewardRsp.RspCode.ToString(CultureInfo.InvariantCulture), rewardRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = rewardRsp.AuthCode,
                BalanceAmount = rewardRsp.BalanceAmt,
                PointsBalanceAmount = rewardRsp.PointsBalanceAmt,
                ResponseCode = rewardRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = rewardRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Sale</b> transaction is used to record a sale against the gift card. If successful, the card
        /// amount is subtracted from the gift card amount.
        /// </summary>
        /// <param name="giftCard">The gift card information.</param>
        /// <param name="amount">The amount of purchase to be used in rewards calculation.</param>
        /// <param name="currency">Identifies the currency of a financial transaction ("usd" or "points")</param>
        /// <param name="gratuity">The portion of the purchase amount that is a gratuity</param>
        /// <param name="tax">The portion of the purchase amount that is tax</param>
        /// <returns>The <see cref="HpsGiftCardReward"/>.</returns>
        public HpsGiftCardSale Sale(HpsGiftCard giftCard, decimal amount, string currency = "usd",
            decimal? gratuity = null, decimal? tax = null)
        {
            currency = currency.ToLower();
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardSaleReqType
                {
                    Block1 = new GiftCardSaleReqBlock1Type
                    {
                        Amt = amount,
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardSale
            };

            var block = ((PosGiftCardSaleReqType)transaction.Item).Block1;
            if (currency == "usd" || currency == "points")
            {
                block.Currency = currency == "usd" ? currencyType.USD : currencyType.POINTS;
                block.CurrencySpecified = true;
            }

            if (gratuity != null)
            {
                block.GratuityAmtInfo = gratuity.Value;
                block.GratuityAmtInfoSpecified = true;
            }

            if (tax != null)
            {
                block.TaxAmtInfo = tax.Value;
                block.TaxAmtInfoSpecified = true;
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardSale);

            var saleRsp = (PosGiftCardSaleRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                saleRsp.RspCode.ToString(CultureInfo.InvariantCulture), saleRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardSale
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = saleRsp.AuthCode,
                BalanceAmount = saleRsp.BalanceAmt,
                SplitTenderCardAmount = saleRsp.SplitTenderCardAmt,
                SplitTenderBalanceDue = saleRsp.SplitTenderBalanceDueAmt,
                PointsBalanceAmount = saleRsp.PointsBalanceAmt,
                ResponseCode = saleRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = saleRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Void</b> transaction is used to cancel a previously approved Sale, Activate, Add Value, Deactivate, or Replace.
        /// If successful, the gift card is credited with the amount of the sale.
        /// </summary>
        /// <param name="transactionId">Transaction identifier assigned by Portico Gateway of the Sale transaction to void.</param>
        /// <returns>The <see cref="HpsGiftCardReward"/>.</returns>
        public HpsGiftCardResponse VoidTransaction(int transactionId)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardVoidReqType
                {
                    Block1 = new GiftCardVoidReqBlock1Type
                    {
                        GatewayTxnId = transactionId
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardVoid
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardVoid);

            var voidRsp = (PosGiftCardVoidRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                voidRsp.RspCode.ToString(CultureInfo.InvariantCulture), voidRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = voidRsp.AuthCode,
                BalanceAmount = voidRsp.BalanceAmt,
                PointsBalanceAmount = voidRsp.PointsBalanceAmt,
                Notes = voidRsp.Notes,
                ResponseCode = voidRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = voidRsp.RspText
            };

            return response;
        }

        /// <summary>
        /// A <b>Reverse</b> transaction is used to cancel a previously approved Sale, Activate, or AddValue
        /// from the current open batch. If successful, the gift card balance is restored to the amount prior
        /// to the transaction being reversed.
        /// </summary>
        /// <param name="giftCard">The gift card information.</param>
        /// <param name="amount">Amount of the transaction to be reversed.</param>
        /// <param name="currency">Identifies the currency of a financial transaction ("usd" or "points")</param>
        /// <returns>The <see cref="HpsGiftCardReward"/>.</returns>
        public HpsGiftCardResponse Reverse(HpsGiftCard giftCard, decimal amount, string currency = "usd")
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardReversalReqType
                {
                    Block1 = new GiftCardReversalReqBlock1Type
                    {
                        Amt = amount,
                        CardData = HydrateGiftCardData(giftCard)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardReversal
            };

            /* Submit the transaction. */
            return SubmitReversal(transaction);
        }

        /// <summary>
        /// A <b>Reverse</b> transaction is used to cancel a previously approved Sale, Activate, or AddValue
        /// from the current open batch. If successful, the gift card balance is restored to the amount prior
        /// to the transaction being reversed.
        /// </summary>
        /// <param name="transactionId">Transaction identifier assigned by Portico Gateway of the transaction to be reversed.</param>
        /// <param name="amount">Amount of the transaction to be reversed.</param>
        /// <param name="currency">Identifies the currency of a financial transaction ("usd" or "points")</param>
        /// <returns>The <see cref="HpsGiftCardReward"/>.</returns>
        public HpsGiftCardResponse Reverse(int transactionId, decimal amount, string currency = "usd")
        {
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosGiftCardReversalReqType
                {
                    Block1 = new GiftCardReversalReqBlock1Type
                    {
                        Amt = amount,
                        GatewayTxnId = transactionId,
                        GatewayTxnIdSpecified = true
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardReversal
            };

            /* Submit the transaction. */
            return SubmitReversal(transaction);
        }

        private HpsGiftCardResponse SubmitAddValue(PosRequestVer10Transaction transaction)
        {
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardAddValue);

            var addValueRsp = (PosGiftCardAddValueRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                addValueRsp.RspCode.ToString(CultureInfo.InvariantCulture), addValueRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var addValue = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = addValueRsp.AuthCode,
                BalanceAmount = addValueRsp.BalanceAmt,
                PointsBalanceAmount = addValueRsp.PointsBalanceAmt,
                Rewards = addValueRsp.Rewards,
                Notes = addValueRsp.Notes,
                ResponseCode = addValueRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = addValueRsp.RspText
            };

            return addValue;
        }

        /// <summary>Helper method for activate transactions (supporting multiple activation methods).</summary>
        /// <param name="transaction">The gateway response.</param>
        /// <returns>The <see cref="HpsGiftCardActivate"/>.</returns>
        private HpsGiftCardResponse SubmitActivation(PosRequestVer10Transaction transaction)
        {
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardActivate);

            var activationRsp = (PosGiftCardActivateRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                activationRsp.RspCode.ToString(CultureInfo.InvariantCulture), activationRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardTransactionResponse). */
            var activation = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = activationRsp.AuthCode,
                BalanceAmount = activationRsp.BalanceAmt,
                PointsBalanceAmount = activationRsp.PointsBalanceAmt,
                Rewards = activationRsp.Rewards,
                Notes = activationRsp.Notes,
                ResponseCode = activationRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = activationRsp.RspText
            };

            return activation;
        }

        /// <summary>Helper method for reversal transactions (supporting multiple reversal methods).</summary>
        /// <param name="transaction">The gateway response.</param>
        /// <returns>The <see cref="HpsGiftCardActivate"/>.</returns>
        private HpsGiftCardResponse SubmitReversal(PosRequestVer10Transaction transaction)
        {
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReversal);

            var reversalRsp = (PosGiftCardReversalRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                reversalRsp.RspCode.ToString(CultureInfo.InvariantCulture), reversalRsp.RspText);

            /* Start to fill out a new transaction response. */
            var reversal = new HpsGiftCardResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = reversalRsp.AuthCode,
                BalanceAmount = reversalRsp.BalanceAmt,
                ResponseCode = reversalRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = reversalRsp.RspText
            };

            return reversal;
        }

        private static GiftCardDataType HydrateGiftCardData(HpsGiftCard giftCard)
        {
            return new GiftCardDataType
            {
                Item = giftCard.Value,
                ItemElementName = giftCard.ValueType,
                EncryptionData = (giftCard.EncryptionData == null) ? null : new EncryptionDataType
                {
                    EncryptedTrackNumber = giftCard.EncryptionData.EncryptedTrackNumber,
                    KSN = giftCard.EncryptionData.Ksn,
                    KTB = giftCard.EncryptionData.Ktb,
                    Version = giftCard.EncryptionData.Version
                }
            };
        }
    }
}
