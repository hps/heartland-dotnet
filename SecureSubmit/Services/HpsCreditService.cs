using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using System;
using System.Collections.Generic;

namespace SecureSubmit.Services
{
    /// <summary>The HPS charge service.</summary>
    public class HpsCreditService : HpsSoapGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsCreditService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        public HpsCreditService(IHpsServicesConfig config = null)
            : base(config)
        {
        }

        /// <summary>Gets an HPS transaction given a Transaction ID.</summary>
        /// <param name="transactionId">The Transaction ID for the transaction.</param>
        /// <returns>The HPS report transaction.</returns>
        public HpsReportTransactionDetails Get(long transactionId)
        {
            if (transactionId <= 0)
            {
                throw new ArgumentException(Resource.Invalid_Transaction_ID, "transactionId");
            }

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                ItemElementName = ItemChoiceType1.ReportTxnDetail,
                Item = new PosReportTxnDetailReqType
                {
                    TxnId = transactionId
                }
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.ReportTxnDetail);

            /* Start to fill out a new transaction response (HpsCharge). */
            var reportResponse = (PosReportTxnDetailRspType)rsp.Transaction.Item;
            return new HpsReportTransactionDetails
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = reportResponse.GatewayTxnId,
                ClientTransactionId = reportResponse.ClientTxnId,
                OriginalTransactionId = reportResponse.OriginalGatewayTxnId,
                AuthorizedAmount = reportResponse.Data.AuthAmt,
                SettlementAmount = reportResponse.Data.SettlementAmt,
                AuthorizationCode = reportResponse.Data.AuthCode,
                AvsResultCode = reportResponse.Data.AVSRsltCode,
                AvsResultText = reportResponse.Data.AVSRsltText,
                CardType = reportResponse.Data.CardType,
                MaskedCardNumber = reportResponse.Data.MaskedCardNbr,
                Descriptor = reportResponse.Data.TxnDescriptor,
                TransactionType = HpsTransaction.ServiceNameToTransactionType(reportResponse.ServiceName),
                TransactionUtcDate = reportResponse.ReqUtcDT,
                CpcIndicator = reportResponse.Data.CPCInd,
                CvvResultCode = reportResponse.Data.CVVRsltCode,
                CvvResultText = reportResponse.Data.CVVRsltText,
                ReferenceNumber = reportResponse.Data.RefNbr,
                ResponseCode = reportResponse.Data.RspCode,
                ResponseText = reportResponse.Data.RspText,
                TransactionStatus = reportResponse.Data.TxnStatus,

                TokenData = string.IsNullOrEmpty(reportResponse.Data.TokenizationMsg) ? null : new HpsTokenData
                {
                    TokenRspMsg = reportResponse.Data.TokenizationMsg
                },
                Exceptions = (reportResponse.GatewayRspCode != 0 || reportResponse.Data.RspCode != "00") ? new HpsChargeExceptions
                {
                    GatewayException = HpsGatewayResponseValidation.GetException(reportResponse.GatewayRspCode, reportResponse.GatewayRspMsg),
                    IssuerException = HpsIssuerResponseValidation.GetException(reportResponse.GatewayTxnId, reportResponse.Data.RspCode, reportResponse.Data.RspText)
                }
                : null,
                CustomerId = reportResponse.Data.AdditionalTxnFields != null ? reportResponse.Data.AdditionalTxnFields.CustomerID : string.Empty,
                InvoiceNumber = reportResponse.Data.AdditionalTxnFields != null ? reportResponse.Data.AdditionalTxnFields.InvoiceNbr : string.Empty,
                Memo = reportResponse.Data.AdditionalTxnFields != null ? reportResponse.Data.AdditionalTxnFields.Description : string.Empty,
                ConvenienceAmount = reportResponse.Data.ConvenienceAmtInfoSpecified ? reportResponse.Data.ConvenienceAmtInfo : 0m,
                ShippingAmount = reportResponse.Data.ShippingAmtInfoSpecified ? reportResponse.Data.ShippingAmtInfo : 0m

            };
        }

        /// <summary>Gets a of transaction summaries between a set of dates and filtered if specified.</summary>
        /// <param name="utcStart">Start date.</param>
        /// <param name="utcEnd">End date.</param>
        /// <param name="filterBy">Filter the result set.</param>
        /// <returns>The list of transaction summaries.</returns>
        public List<HpsReportTransactionSummary> List(DateTime? utcStart = null, DateTime? utcEnd = null, HpsTransactionType? filterBy = null)
        {
            HpsInputValidation.CheckDateNotFuture(utcStart);
            HpsInputValidation.CheckDateNotFuture(utcEnd);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                ItemElementName = ItemChoiceType1.ReportActivity,
                Item = new PosReportActivityReqType
                {
                    DeviceIdSpecified = false
                }
            };

            var item = (PosReportActivityReqType)transaction.Item;
            if (utcStart != null)
            {
                item.RptStartUtcDT = utcStart.Value;
                item.RptStartUtcDTSpecified = true;
            }

            if (utcEnd != null)
            {
                item.RptEndUtcDT = utcEnd.Value;
                item.RptEndUtcDTSpecified = true;
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.ReportActivity);

            var reportResponse = (PosReportActivityRspType)rsp.Transaction.Item;
            var serviceName = filterBy.HasValue ? HpsTransaction.TransactionTypeToServiceName(filterBy.Value) : string.Empty;
            var transactionList = new List<HpsReportTransactionSummary>();

            if (reportResponse.Details != null)
            {
                foreach (var charge in reportResponse.Details)
                {
                    if (!filterBy.HasValue || charge.ServiceName == serviceName)
                    {
                        transactionList.Add(new HpsReportTransactionSummary
                        {
                            TransactionId = charge.GatewayTxnId,
                            OriginalTransactionId = charge.OriginalGatewayTxnId,
                            MaskedCardNumber = charge.MaskedCardNbr,
                            ResponseCode = charge.IssuerRspCode,
                            ResponseText = charge.IssuerRspText,
                            Amount = charge.Amt,
                            SettlementAmount = charge.SettlementAmt,
                            TransactionUtcDate = charge.TxnUtcDT,
                            TransactionType = filterBy.HasValue ? filterBy : HpsTransaction.ServiceNameToTransactionType(charge.ServiceName),
                            Exceptions = (charge.GatewayRspCode != 0 || (!string.IsNullOrEmpty(charge.IssuerRspCode) && charge.IssuerRspCode != "00")) ? new HpsChargeExceptions
                            {
                                GatewayException = HpsGatewayResponseValidation.GetException(charge.GatewayRspCode, charge.GatewayRspMsg),
                                IssuerException = HpsIssuerResponseValidation.GetException(charge.GatewayTxnId, charge.IssuerRspCode, charge.IssuerRspText)
                            }
                            : null
                        });
                    }
                }
            }

            return transactionList;
        }

        /// <summary>
        /// The <b>credit sale</b> transaction authorizes a sale purchased with a credit card. The
        /// authorization in place in the current open batch (should auto-close for e-commerce
        /// transactions). If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="card">The credit card information.</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token.</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="encryptionData">The encryption data.</param>
        /// <param name="gratuity">The gratuity aamount.</param>
        /// <param name="directMarketData">The direct market data.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsCharge Charge(decimal amount, string currency, HpsCreditCard card, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, HpsEncryptionData encryptionData = null, decimal gratuity = 0,
            HpsDirectMarketData directMarketData = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditSaleReqType
                {
                    Block1 = new CreditSaleReqBlock1Type
                    {
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        CardData = new CardDataType
                        {
                            EncryptionData = HydrateEncryptionData(encryptionData),
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardManualEntry(card)
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details),
                        TxnDescriptor = descriptor,
                        DirectMktData = HydrateDirectMktData(directMarketData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditSale
            };

            /* Submit the transaction. */
            return SubmitCharge(transaction, amount, currency, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>
        /// The <b>credit sale</b> transaction authorizes a sale purchased with a credit card. The
        /// authorization in place in the current open batch (should auto-close for e-commerce
        /// transactions). If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="gratuity">The gratuity amount.</param>
        /// <param name="directMarketData">The direct market data.</param>
        /// <param name="tokenExpMonth">The updated expiration month.</param>
        /// <param name="tokenExpYear">The update expiration year.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsCharge Charge(decimal amount, string currency, string token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, decimal gratuity = 0, HpsDirectMarketData directMarketData = null,
            int? tokenExpMonth = null, int? tokenExpYear = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            return Charge(
                amount,
                currency,
                new HpsTokenData { TokenValue = token, ExpMonth = tokenExpMonth, ExpYear = tokenExpYear },
                cardHolder,
                requestMultiUseToken,
                descriptor,
                allowPartialAuth,
                details,
                gratuity,
                directMarketData,
                convenienceAmt,
                shippingAmt
            );
        }

        /// <summary>
        /// The <b>credit sale</b> transaction authorizes a sale purchased with a credit card. The
        /// authorization in place in the current open batch (should auto-close for e-commerce
        /// transactions). If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="gratuity">The gratuity amount.</param>
        /// <param name="directMarketData">The direct market data.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsCharge Charge(decimal amount, string currency, HpsTokenData token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, decimal gratuity = 0, HpsDirectMarketData directMarketData = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditSaleReqType
                {
                    Block1 = new CreditSaleReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateTokenData(token)
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details),
                        TxnDescriptor = descriptor,
                        DirectMktData = HydrateDirectMktData(directMarketData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditSale
            };

            return SubmitCharge(transaction, amount, currency, (details == null) ? null : details.ClientTransactionId);
        }

        public HpsCharge Charge(decimal amount, string currency, HpsTrackData trackData, HpsEncryptionData encryptionData = null,
            decimal gratuity = 0, bool allowPartialAuthorization = false, bool requestMultiUseToken = false,
            HpsDirectMarketData directMarketData = null, HpsTagDataType tagData = null, bool allowDuplicates = false, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditSaleReqType
                {
                    Block1 = new CreditSaleReqBlock1Type
                    {
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        AllowPartialAuth = allowPartialAuthorization ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                        AllowDupSpecified = true,
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardTrackData(trackData),
                            EncryptionData = HydrateEncryptionData(encryptionData)
                        },
                        DirectMktData = HydrateDirectMktData(directMarketData),                        
                        TagData = HydrateTagData(tagData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditSale
            };

            return SubmitCharge(transaction, amount, currency);
        }

        /// <summary>
        /// A <b>credit account verify</b> transaction is used to verify that the account is in good standing
        /// with the issuer. This is a zero dollar transaction with no associated authorization. Since VISA and
        /// other issuers have started assessing penalties for one dollar authorizations, this provides a way for
        /// merchants to accomplish the same task while avoiding these penalties.
        /// </summary>
        /// <param name="card">The credit card information.</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token.</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsAccountVerify Verify(HpsCreditCard card, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, long? clientTransactionId = null)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAccountVerifyReqType
                {
                    Block1 = new CreditAccountVerifyBlock1Type
                    {
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardManualEntry(card)
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAccountVerify
            };

            return SubmitVerify(transaction, clientTransactionId);
        }

        /// <summary>
        /// A <b>credit account verify</b> transaction is used to verify that the account is in good standing
        /// with the issuer. This is a zero dollar transaction with no associated authorization. Since VISA and
        /// other issuers have started assessing penalties for one dollar authorizations, this provides a way for
        /// merchants to accomplish the same task while avoiding these penalties.
        /// </summary>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token.</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <param name="tokenExpMonth">The updated expiration month.</param>
        /// <param name="tokenExpYear">The update expiration year.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsAccountVerify Verify(string token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, long? clientTransactionId = null,
            int? tokenExpMonth = null, int? tokenExpYear = null)
        {
            return Verify(
                new HpsTokenData { TokenValue = token, ExpMonth = tokenExpMonth, ExpYear = tokenExpYear },
                cardHolder,
                requestMultiUseToken,
                clientTransactionId
            );
        }

        /// <summary>
        /// A <b>credit account verify</b> transaction is used to verify that the account is in good standing
        /// with the issuer. This is a zero dollar transaction with no associated authorization. Since VISA and
        /// other issuers have started assessing penalties for one dollar authorizations, this provides a way for
        /// merchants to accomplish the same task while avoiding these penalties.
        /// </summary>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token.</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <returns>The <see cref="HpsCharge"/>.</returns>
        public HpsAccountVerify Verify(HpsTokenData token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, long? clientTransactionId = null)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAccountVerifyReqType
                {
                    Block1 = new CreditAccountVerifyBlock1Type
                    {
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateTokenData(token)
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAccountVerify
            };

            return SubmitVerify(transaction, clientTransactionId);
        }

        /// <summary>
        /// A <b>credit account verify</b> transaction is used to verify that the account is in good standing
        /// with the issuer. This is a zero dollar transaction with no associated authorization. Since VISA and
        /// other issuers have started assessing penalties for one dollar authorizations, this provides a way for
        /// merchants to accomplish the same task while avoiding these penalties.
        /// </summary>
        /// <param name="trackData">The CC track data.</param>
        /// <param name="encryptionData">Optional encryption data.</param>
        /// <param name="requestMultiUseToken">Request a multi-use token.</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <param name="emvData">Optional EMV card data</param>
        /// <returns>The <see cref="HpsAccountVerify"/>.</returns>
        public HpsAccountVerify Verify(HpsTrackData trackData, HpsEncryptionData encryptionData = null,
            bool requestMultiUseToken = false, long? clientTransactionId = null, HpsTagDataType tagData = null)
        {
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAccountVerifyReqType
                {
                    Block1 = new CreditAccountVerifyBlock1Type
                    {
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardTrackData(trackData),
                            EncryptionData = HydrateEncryptionData(encryptionData)
                        },                        
                        TagData = HydrateTagData(tagData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAccountVerify
            };

            return SubmitVerify(transaction, clientTransactionId);
        }

        /// <summary>
        /// A <b>credit authorization</b> transaction authorizes a credit card transaction. The authorization is NOT placed
        /// in the batch. The <b>credit authorization</b> transaction can be committed by using the capture method.
        /// </summary>
        /// <param name="amount">Amount to verify.</param>
        /// <param name="currency">Currency to use (e.g. "USD").</param>
        /// <param name="card">The credit card information.</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="gratuity">The gratuity amount.</param>
        /// <returns>The <see cref="HpsAuthorization"/>.</returns>
        public HpsAuthorization Authorize(decimal amount, string currency, HpsCreditCard card, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, decimal gratuity = 0, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAuthReqType
                {
                    Block1 = new CreditAuthReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardManualEntry(card)
                        },
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details),
                        TxnDescriptor = descriptor
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAuth
            };

            return SubmitAuthorize(transaction, amount, currency, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>
        /// A <b>credit authorization</b> transaction authorizes a credit card transaction. The authorization is NOT placed
        /// in the batch. The <b>credit authorization</b> transaction can be committed by using the capture method.
        /// </summary>
        /// <param name="amount">Amount to verify.</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="gratuity">The gratuity.</param>
        /// <param name="tokenExpMonth">The updated expiration month.</param>
        /// <param name="tokenExpYear">The update expiration year.</param>
        /// <returns>The <see cref="HpsAuthorization"/>.</returns>
        public HpsAuthorization Authorize(decimal amount, string currency, string token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, decimal gratuity = 0, int? tokenExpMonth = null, int? tokenExpYear = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            return Authorize(
                amount,
                currency,
                new HpsTokenData { TokenValue = token, ExpMonth = tokenExpMonth, ExpYear = tokenExpYear },
                cardHolder,
                requestMultiUseToken,
                descriptor,
                allowPartialAuth,
                details,
                gratuity,
                convenienceAmt,
                shippingAmt
            );
        }

        /// <summary>
        /// A <b>credit authorization</b> transaction authorizes a credit card transaction. The authorization is NOT placed
        /// in the batch. The <b>credit authorization</b> transaction can be committed by using the capture method.
        /// </summary>
        /// <param name="amount">Amount to verify.</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="requestMultiUseToken">Request a multi-use token</param>
        /// <param name="descriptor">Transaction description that is concatenated to a configurable
        /// merchant DBA name. The resulting string is sent to the card issuer for the Merchant Name.</param>
        /// <param name="allowPartialAuth">Indicated whether partial authorization is supported.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="gratuity">The gratuity.</param>
        /// <returns>The <see cref="HpsAuthorization"/>.</returns>
        public HpsAuthorization Authorize(decimal amount, string currency, HpsTokenData token, HpsCardHolder cardHolder = null,
            bool requestMultiUseToken = false, string descriptor = null, bool allowPartialAuth = false,
            HpsTransactionDetails details = null, decimal gratuity = 0, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAuthReqType
                {
                    Block1 = new CreditAuthReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateTokenData(token)
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details),
                        TxnDescriptor = descriptor
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAuth
            };

            return SubmitAuthorize(transaction, amount, currency, (details == null) ? null : details.ClientTransactionId);
        }

        public HpsAuthorization Authorize(decimal amount, string currency, HpsTrackData trackData, HpsEncryptionData encryptionData = null,
            decimal gratuity = 0, bool allowPartialAuthorization = false, bool requestMultiUseToken = false, HpsDirectMarketData directMarketData = null, HpsTagDataType tagData = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAuthReqType
                {
                    Block1 = new CreditAuthReqBlock1Type
                    {
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        AllowPartialAuth = allowPartialAuthorization ? booleanType.Y : booleanType.N,
                        AllowPartialAuthSpecified = true,
                        CardData = new CardDataType
                        {
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardTrackData(trackData),
                            EncryptionData = HydrateEncryptionData(encryptionData)
                        },
                        DirectMktData = HydrateDirectMktData(directMarketData),                        
                        TagData = HydrateTagData(tagData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditAuth
            };

            return SubmitAuthorize(transaction, amount, currency);
        }

        /// <summary>
        /// A <b>Capture</b> transaction adds a previous authorization transaction to the current open
        /// batch. If a batch is not open, this transaction will create one. 
        /// </summary>
        /// <param name="transactionId">The authorization transaction ID.</param>
        /// <param name="amount">An amount to charge (optional). Used if different from original authorization.</param>
        /// <param name="gratuity">The gratuity amount (optional).</param>
        /// <param name="clientTransactionId">The optional client transaction ID.</param>
        /// <param name="directMarketData">The direct market data.</param>
        /// <returns>The details of the charge captured.</returns>
        public HpsReportTransactionDetails Capture(long transactionId, decimal? amount = null, decimal? gratuity = null,
            long? clientTransactionId = null, HpsDirectMarketData directMarketData = null)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAddToBatchReqType
                {
                    GatewayTxnId = transactionId
                },
                ItemElementName = ItemChoiceType1.CreditAddToBatch
            };

            var request = (PosCreditAddToBatchReqType)transaction.Item;
            request.AmtSpecified = amount.HasValue;
            if (amount.HasValue)
            {
                request.Amt = amount.Value;
            }

            request.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (gratuity.HasValue)
            {
                request.GratuityAmtInfo = gratuity.Value;
            }

            request.DirectMktData = HydrateDirectMktData(directMarketData);

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditAddToBatch);

            return Get(transactionId);
        }

        /// <summary>
        /// The <b>credit return transaction</b> returns funds to the cardholder. The transaction is generally used
        /// as a counterpart to a credit card transaction that needs to be reversed, and the batch containing
        /// the original transaction has already been closed. The credit return transaction is placed in the
        /// current open batch. If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="card">The credit card information.</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="details">The transaction details.</param>
        /// <returns>The <see cref="HpsRefund"/>.</returns>
        public HpsRefund Refund(decimal amount, string currency, HpsCreditCard card, HpsCardHolder cardHolder = null,
            HpsTransactionDetails details = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReturnReqType
                {
                    Block1 = new CreditReturnReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            Item = HydrateCardManualEntry(card),
                        },
                        Amt = amount,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReturn
            };

            return SubmitRefund(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>
        /// The <b>credit return transaction</b> returns funds to the cardholder. The transaction is generally used
        /// as a counterpart to a credit card transaction that needs to be reversed, and the batch containing
        /// the original transaction has already been closed. The credit return transaction is placed in the
        /// current open batch. If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="details">The transaction details.</param>
        /// <returns>The <see cref="HpsRefund"/>.</returns>
        public HpsRefund Refund(decimal amount, string currency, string token, HpsCardHolder cardHolder = null,
            HpsTransactionDetails details = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReturnReqType
                {
                    Block1 = new CreditReturnReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            }
                        },
                        Amt = amount,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReturn
            };

            return SubmitRefund(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>
        /// The <b>credit return transaction</b> returns funds to the cardholder. The transaction is generally used
        /// as a counterpart to a credit card transaction that needs to be reversed, and the batch containing
        /// the original transaction has already been closed. The credit return transaction is placed in the
        /// current open batch. If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="details">The transaction details.</param>
        /// <returns>The <see cref="HpsRefund"/>.</returns>
        public HpsRefund Refund(decimal amount, string currency, long transactionId, HpsCardHolder cardHolder = null,
            HpsTransactionDetails details = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReturnReqType
                {
                    Block1 = new CreditReturnReqBlock1Type
                    {
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        GatewayTxnId = transactionId,
                        GatewayTxnIdSpecified = true,
                        Amt = amount,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReturn
            };

            return SubmitRefund(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>A <b>reverse</b> transaction reverses a <b>Charge</b> or <b>Authorize</b> transaction
        /// from the active open authorizations or current open batch.</summary>
        /// <param name="transactionId">The transaction ID of charge to reverse.</param>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="authorizedAmount">Settlement amount or new authorized amount after reversal occures.</param>
        /// <returns>The <see cref="HpsReversal"/>.</returns>
        public HpsReversal Reverse(long transactionId, decimal amount, string currency, HpsTransactionDetails details = null,
            decimal? authorizedAmount = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReversalReqType
                {
                    Block1 = new CreditReversalReqBlock1Type
                    {
                        GatewayTxnId = transactionId,
                        GatewayTxnIdSpecified = true,
                        Amt = amount,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReversal
            };


            if (!authorizedAmount.HasValue)
                return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);

            var block = ((PosCreditReversalReqType)transaction.Item).Block1;
            block.AuthAmt = authorizedAmount.Value;
            block.AuthAmtSpecified = true;
            return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>A <b>reverse</b> transaction reverses a <b>Charge</b> or <b>Authorize</b> transaction
        /// from the active open authorizations or current open batch.</summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="token">The secure token.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="authorizedAmount">Settlement amount or new authorized amount after reversal occures.</param>
        /// <returns>The <see cref="HpsReversal"/>.</returns>
        public HpsReversal Reverse(decimal amount, string currency, string token, HpsTransactionDetails details = null,
            decimal? authorizedAmount = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReversalReqType
                {
                    Block1 = new CreditReversalReqBlock1Type
                    {
                        Amt = amount,
                        CardData = new CardDataType
                        {
                            Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            }
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReversal
            };

            if (!authorizedAmount.HasValue)
                return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);

            var block = ((PosCreditReversalReqType)transaction.Item).Block1;
            block.AuthAmt = authorizedAmount.Value;
            block.AuthAmtSpecified = true;
            return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>A <b>reverse</b> transaction reverses a <b>Charge</b> or <b>Authorize</b> transaction
        /// from the active open authorizations or current open batch.</summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="card">The card for which to reverse a charge.</param>
        /// <param name="details">The transaction details.</param>
        /// <param name="authorizedAmount">Settlement amount or new authorized amount after reversal occures.</param>
        /// <returns>The <see cref="HpsReversal"/>.</returns>
        public HpsReversal Reverse(decimal amount, string currency, HpsCreditCard card, HpsTransactionDetails details = null,
            decimal? authorizedAmount = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReversalReqType
                {
                    Block1 = new CreditReversalReqBlock1Type
                    {
                        Amt = amount,
                        CardData = new CardDataType
                        {
                            Item = HydrateCardManualEntry(card)
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReversal
            };

            if (!authorizedAmount.HasValue)
                return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);

            var block = ((PosCreditReversalReqType)transaction.Item).Block1;
            block.AuthAmt = authorizedAmount.Value;
            block.AuthAmtSpecified = true;
            return SubmitReverse(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>
        /// The <b>credit return transaction</b> returns funds to the cardholder. The transaction is generally used
        /// as a counterpart to a credit card transaction that needs to be reversed, and the batch containing
        /// the original transaction has already been closed. The credit return transaction is placed in the
        /// current open batch. If a batch is not open, this transaction will create an open batch.
        /// </summary>
        /// <param name="amount">The amount (in dollars).</param>
        /// <param name="currency">The currency (3-letter ISO code for currency).</param>
        /// <param name="trackData">The card data</param>
        /// <param name="encryptionData">encryption data</param>
        /// <param name="cardHolder">The card holder information (used for AVS).</param>
        /// <param name="details">The transaction details.</param>
        /// <returns>The <see cref="HpsRefund"/>.</returns>
        public HpsRefund Refund(decimal amount, string currency, HpsTrackData trackData, HpsEncryptionData encryptionData = null,
            HpsCardHolder cardHolder = null, HpsTransactionDetails details = null)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditReturnReqType
                {
                    Block1 = new CreditReturnReqBlock1Type
                    {
                        AllowDup = booleanType.Y,
                        AllowDupSpecified = true,
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        CardData = new CardDataType
                        {
                            Item = HydrateCardTrackData(trackData),
                            EncryptionData = HydrateEncryptionData(encryptionData)
                        },
                        Amt = amount,
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditReturn
            };

            return SubmitRefund(transaction, (details == null) ? null : details.ClientTransactionId);
        }

        /// <summary>A <b>credit void</b> transaction is used to inactivate a transaction.
        /// The transaction must be an <b>Authorize</b>, <b>Charge</b> or <b>Return</b>.
        /// The transaction must be active in order to be voided. <b>Authorize</b>
        /// transactions do <b>not</b> have to be associated with a batch to be voided.
        /// Transactions may be voided after they are associated with a batch as long as
        /// the batch is not closed.<br/><br/>
        /// Note: If the batch containing the original transaction has been closed, a
        /// <b>Return</b> transaction may be used to credit the cardholder.<br/><br/>
        /// Note: If a transaction has been returned, it cannot be voided.</summary>
        /// <param name="transactionId">The transaction ID of charge to void.</param>
        /// <param name="clientTransactionId">The optional client transaction ID.</param>
        /// <returns>The <see cref="HpsTransaction"/>.</returns>
        public HpsTransaction Void(long transactionId, long? clientTransactionId = null)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditVoidReqType
                {
                    GatewayTxnId = transactionId
                },
                ItemElementName = ItemChoiceType1.CreditVoid
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditVoid);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        /// <summary>An <b>edit</b> transaction changes the data on a previously approved
        /// <b>Charge</b> or <b>Authorize</b> transaction.<br/><br/>
        /// <b>Note:</b> When the settlement amount of a transaction is altered with this 
        /// service, the Portico Gateway does not send an update to the Issuer. For example, 
        /// if the settlement amount of a transaction is reduced, a reversal for the 
        /// difference is not sent. Likewise, if the amount is increased, an additional 
        /// authorization is not sent. These additional operations are the responsibility 
        /// of the POS. Additional features like this are being considered for future 
        /// releases of the Portico Gateway.</summary>
        /// <param name="transactionId">The transaction ID of charge to void.</param>
        /// <param name="amount">If not null, revises (replaces) the authorized amount of 
        /// the original auth. If null, does not affect the authorized amount of the original auth.</param>
        /// <param name="gratuity">If not null, revises (replaces) the gratuity amount information 
        /// of the original auth. If null, does not affect the gratuity amount information, if any, 
        /// of the original auth. This element is for <b>informational purposes only</b> and does 
        /// not affect the authorized amount.</param>
        /// <param name="clientTransactionId">The optional client transaction ID.</param>
        /// <returns>The <see cref="HpsTransaction"/>.</returns>
        public HpsTransaction Edit(long transactionId, decimal? amount = null, decimal? gratuity = null, long? clientTransactionId = null)
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditTxnEditReqType
                {
                    GatewayTxnId = transactionId
                },
                ItemElementName = ItemChoiceType1.CreditTxnEdit
            };

            var request = (PosCreditTxnEditReqType)transaction.Item;

            request.AmtSpecified = amount.HasValue;
            if (amount.HasValue)
            {
                request.Amt = amount.Value;
            }

            request.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (gratuity.HasValue)
            {
                request.GratuityAmtInfo = gratuity.Value;
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditTxnEdit);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public HpsTransaction OfflineCharge(decimal amount, string currency, HpsCreditCard card, string offlineAuthCode, bool allowDuplicates = false,
            bool cpcRequest = false, HpsCardHolder cardHolder = null, bool requestMultiUseToken = false, HpsTransactionDetails details = null,
            HpsEncryptionData encryptionData = null, decimal gratuity = 0, decimal surcharge = 0, long? clientTransactionId = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditOfflineSaleReqType
                {
                    Block1 = new CreditOfflineSaleReqBlock1Type
                    {
                        CardHolderData = cardHolder == null ? null : HydrateCardHolderData(cardHolder),
                        AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                        AllowDupSpecified = true,
                        Amt = amount,
                        CPCReq = cpcRequest ? booleanType.Y : booleanType.N,
                        CPCReqSpecified = true,
                        OfflineAuthCode = offlineAuthCode,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,                       
                        SurchargeAmtInfo = surcharge,
                        SurchargeAmtInfoSpecified = surcharge != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        CardData = new CardDataType
                        {
                            EncryptionData = HydrateEncryptionData(encryptionData),
                            TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N,
                            Item = HydrateCardManualEntry(card)
                        },
                        AdditionalTxnFields = HydrateAdditionalTxnFields(details)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditOfflineSale
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditOfflineSale);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public HpsTransaction OfflineCharge(decimal amount, string currency, HpsTrackData trackData, HpsEncryptionData encryptionData = null,
            decimal gratuity = 0, decimal surcharge = 0, long? clientTransactionId = null, HpsTagDataType tagData = null, decimal convenienceAmt = 0, decimal shippingAmt = 0)
        {
            HpsInputValidation.CheckAmount(amount);
            HpsInputValidation.CheckCurrency(currency);
            HpsInputValidation.CheckAmount(convenienceAmt);
            HpsInputValidation.CheckAmount(shippingAmt);
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditOfflineSaleReqType
                {
                    Block1 = new CreditOfflineSaleReqBlock1Type
                    {
                        Amt = amount,
                        GratuityAmtInfo = gratuity,
                        GratuityAmtInfoSpecified = gratuity != 0,
                        SurchargeAmtInfo = surcharge,
                        SurchargeAmtInfoSpecified = surcharge != 0,
                        ConvenienceAmtInfo = convenienceAmt,
                        ConvenienceAmtInfoSpecified = convenienceAmt != 0,
                        ShippingAmtInfo = shippingAmt,
                        ShippingAmtInfoSpecified = shippingAmt != 0,
                        CardData = new CardDataType
                        {
                            Item = HydrateCardTrackData(trackData),
                            EncryptionData = HydrateEncryptionData(encryptionData)
                        },                        
                        TagData = HydrateTagData(tagData)
                    }
                },
                ItemElementName = ItemChoiceType1.CreditOfflineSale
            };

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditOfflineSale);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public HpsTransaction UpdateTokenExpiration(string token, int expMonth, int expYear)
        {
            var transaction = new PosRequestVer10Transaction
            {
                Item = new ManageTokensReqType
                {
                    TokenValue = token,
                    TokenActions = new ManageTokensReqTypeTokenActions
                    {
                        Item = new ManageTokensReqTypeTokenActionsSet
                        {
                            Attribute = new[] 
                            {
                                new NameValuePairType
                                {
                                    Name = "ExpMonth",
                                    Value = expMonth.ToString(CultureInfo.InvariantCulture)
                                },
                                new NameValuePairType
                                {
                                    Name = "ExpYear",
                                    Value = expYear.ToString(CultureInfo.InvariantCulture)
                                }
                            }
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.ManageTokens
            };

            var response = DoTransaction(transaction);
            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(response.Ver10.Header),
                ResponseCode = response.Ver10.Header.GatewayRspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = response.Ver10.Header.GatewayRspMsg,
                TransactionId = response.Ver10.Header.GatewayTxnId
            };
        }

        /// <summary>Submit reversal helper method.</summary>
        /// <param name="transaction">The reversal transaction.</param>
        /// <param name="clientTransactionId">The optional client transaction ID.</param>
        /// <returns>The <see cref="HpsReversal"/>.</returns>
        private HpsReversal SubmitReverse(PosRequestVer10Transaction transaction, long? clientTransactionId = null)
        {
            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditReversal);

            var rspReversal = (AuthRspStatusType)rsp.Transaction.Item;
            return new HpsReversal
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                AvsResultCode = rspReversal.AVSRsltCode,
                AvsResultText = rspReversal.AVSRsltText,
                CpcIndicator = rspReversal.CPCInd,
                CvvResultCode = rspReversal.CVVRsltCode,
                CvvResultText = rspReversal.CVVRsltText,
                ReferenceNumber = rspReversal.RefNbr,
                ResponseCode = rspReversal.RspCode,
                ResponseText = rspReversal.RspText
            };
        }

        /// <summary>Submit a refund transaction.</summary>
        /// <param name="transaction">The refund transaction.</param>
        /// <param name="clientTransactionId">The optional client transaction ID.</param>
        /// <returns>The <see cref="HpsRefund"/>.</returns>
        private HpsRefund SubmitRefund(PosRequestVer10Transaction transaction, long? clientTransactionId = null)
        {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditReturn);

            return new HpsRefund
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        /// <summary>Helper method to submit charge transactions (supporting multiple charge methods).</summary>
        /// <param name="transaction">The gateway response.</param>
        /// <param name="amount">The charge amount (used in case reversal is needed).</param>
        /// <param name="currency">The currency (also used in case reversal is needed).</param>
        /// <param name="clientTransactionId">The client transaction ID (optional).</param>
        /// <returns>The HPS charge object.</returns>
        private HpsCharge SubmitCharge(PosRequestVer10Transaction transaction, decimal amount, string currency, long? clientTransactionId = null)
        {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            ProcessChargeGatewayResponse(rsp, ItemChoiceType2.CreditSale, amount, currency);

            var creditSaleRsp = (AuthRspStatusType)rsp.Transaction.Item;
            ProcessChargeIssuerResponse(creditSaleRsp.RspCode, creditSaleRsp.RspText, rsp.Header.GatewayTxnId, amount, currency);

            /* Start to fill out a new transaction response (HpsCharge). */
            var charge = new HpsCharge
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                AuthorizedAmount = creditSaleRsp.AuthAmt,
                AuthorizationCode = creditSaleRsp.AuthCode,
                AvsResultCode = creditSaleRsp.AVSRsltCode,
                AvsResultText = creditSaleRsp.AVSRsltText,
                CardType = creditSaleRsp.CardType,
                CpcIndicator = creditSaleRsp.CPCInd,
                Descriptor = creditSaleRsp.TxnDescriptor,
                CvvResultCode = creditSaleRsp.CVVRsltCode,
                CvvResultText = creditSaleRsp.CVVRsltText,
                ReferenceNumber = creditSaleRsp.RefNbr,
                ResponseCode = creditSaleRsp.RspCode,
                ResponseText = creditSaleRsp.RspText,
                EMVIssuerResp = creditSaleRsp.EMVIssuerResp
            };

            /* Check to see if the header contains Token Data. If so include it in the response obj. */
            if (rsp.Header.TokenData != null)
            {
                charge.TokenData = new HpsTokenData
                {
                    TokenRspCode = rsp.Header.TokenData.TokenRspCode,
                    TokenRspMsg = rsp.Header.TokenData.TokenRspMsg,
                    TokenValue = rsp.Header.TokenData.TokenValue
                };
            }

            return charge;
        }

        /// <summary>Helper method to submit verify transactions (supporting multiple verify methods).</summary>
        /// <param name="transaction">The verify transaction.</param>
        /// <param name="clientTransactionId">The client transaction ID (optional).</param>
        /// <returns>The HPS verify object.</returns>
        private HpsAccountVerify SubmitVerify(PosRequestVer10Transaction transaction, long? clientTransactionId = null)
        {
            /* Submit the transaction. */
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditAccountVerify);

            var creditVerifyRsp = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, creditVerifyRsp.RspCode, creditVerifyRsp.RspText);

            var accountVerify = new HpsAccountVerify
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                AvsResultCode = creditVerifyRsp.AVSRsltCode,
                ReferenceNumber = creditVerifyRsp.RefNbr,
                ResponseCode = creditVerifyRsp.RspCode,
                ResponseText = creditVerifyRsp.RspText,
                CardType = creditVerifyRsp.CardType,
                CpcIndicator = creditVerifyRsp.CPCInd,
                CvvResultCode = creditVerifyRsp.CVVRsltCode,
                CvvResultText = creditVerifyRsp.CVVRsltText,
                AvsResultText = creditVerifyRsp.AVSRsltText,
                AuthorizationCode = creditVerifyRsp.AuthCode,
                AuthorizedAmount = creditVerifyRsp.AuthAmt,
                EMVIssuerResp = creditVerifyRsp.EMVIssuerResp
            };

            /* Check to see if the header contains Token Data. If so include it in the response obj. */
            if (rsp.Header.TokenData != null)
            {
                accountVerify.TokenData = new HpsTokenData
                {
                    TokenRspCode = rsp.Header.TokenData.TokenRspCode,
                    TokenRspMsg = rsp.Header.TokenData.TokenRspMsg,
                    TokenValue = rsp.Header.TokenData.TokenValue
                };
            }

            return accountVerify;
        }

        /// <summary>Helper method to submit authorize transactions.</summary>
        /// <param name="transaction">The gateway transaction.</param>
        /// <param name="amount">The charge amount (used in case reversal is needed).</param>
        /// <param name="currency">The currency (also used in case reversal is needed).</param>
        /// <param name="clientTransactionId">Optional client transaction ID.</param>
        /// <returns>The HPS charge object.</returns>
        private HpsAuthorization SubmitAuthorize(PosRequestVer10Transaction transaction, decimal amount, string currency, long? clientTransactionId = null)
        {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            ProcessChargeGatewayResponse(rsp, ItemChoiceType2.CreditAuth, amount, currency);

            var porticoAuthRsp = (AuthRspStatusType)rsp.Transaction.Item;
            ProcessChargeIssuerResponse(porticoAuthRsp.RspCode, porticoAuthRsp.RspText, rsp.Header.GatewayTxnId, amount, currency);

            /* Start to fill out a new transaction response (HpsAuth). */
            var auth = new HpsAuthorization
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                AvsResultCode = porticoAuthRsp.AVSRsltCode,
                AvsResultText = porticoAuthRsp.AVSRsltText,
                CvvResultCode = porticoAuthRsp.CVVRsltCode,
                CvvResultText = porticoAuthRsp.CVVRsltText,
                Descriptor = porticoAuthRsp.TxnDescriptor,
                AuthorizationCode = porticoAuthRsp.AuthCode,
                AuthorizedAmount = porticoAuthRsp.AuthAmt,
                ReferenceNumber = porticoAuthRsp.RefNbr,
                ResponseCode = porticoAuthRsp.RspCode,
                ResponseText = porticoAuthRsp.RspText,
                CardType = porticoAuthRsp.CardType,
                CpcIndicator = porticoAuthRsp.CPCInd
            };

            /* Check to see if the header contains Token Data. If so include it in the response obj. */
            if (rsp.Header.TokenData != null)
            {
                auth.TokenData = new HpsTokenData
                {
                    TokenRspCode = rsp.Header.TokenData.TokenRspCode,
                    TokenRspMsg = rsp.Header.TokenData.TokenRspMsg,
                    TokenValue = rsp.Header.TokenData.TokenValue
                };
            }

            return auth;
        }

        /// <summary>Process the issuer response (throw an exception for codes indicative of error, etc.)</summary>
        /// <param name="responseCode">The issuer response code.</param>
        /// <param name="responseText">The issuer response text.</param>
        /// <param name="transactionId">The Gateway transaction ID.</param>
        /// <param name="amount">The amount of the charge, authorization, etc.</param>
        /// <param name="currency">Currency used.</param>
        private void ProcessChargeIssuerResponse(string responseCode, string responseText, long transactionId, decimal amount, string currency)
        {
            if (responseCode == "91")
            {
                try
                {
                    Reverse(transactionId, amount, currency);
                }
                catch (HpsGatewayException e)
                {
                    // if the transaction wasn't found; throw the original timeout exception.
                    if (e.Details.GatewayResponseCode == 3)
                        HpsIssuerResponseValidation.CheckResponse(transactionId, responseCode, responseText);

                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError,
                        "Error occurred while reversing a charge due to HPS issuer time-out.", e);
                }
                catch (Exception e)
                {
                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError,
                        "Error occurred while reversing a charge due to HPS issuer time-out.", e);
                }
            }

            HpsIssuerResponseValidation.CheckResponse(transactionId, responseCode, responseText);
        }

        private void ProcessChargeGatewayResponse(PosResponseVer10 rsp, ItemChoiceType2 expectedResponseType, decimal amount, string currency)
        {
            if (rsp.Header.GatewayRspCode == 0) return;
            if (rsp.Header.GatewayRspCode == 30)
            {
                try
                {
                    Reverse(rsp.Header.GatewayTxnId, amount, currency);
                }
                catch (Exception ex)
                {
                    throw new HpsGatewayException(HpsExceptionCodes.GatewayTimeoutReversalError,
                        "Error occurred while reversing a charge due to HPS gateway time-out.", ex);
                }
            }

            HpsGatewayResponseValidation.CheckResponse(rsp, expectedResponseType);
        }
    }
}