using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class OfflineChargeBuilder : GatewayTransactionBuilder<OfflineChargeBuilder, HpsTransaction>
    {
        public class OfflineChargePaymentTypeBuilder
        {
            private readonly OfflineChargeBuilder _parent;

            public OfflineChargePaymentTypeBuilder(OfflineChargeBuilder parent)
            {
                _parent = parent;
            }

            public OfflineChargeBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public OfflineChargeBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }

            public OfflineChargeBuilder WithTrackData(HpsTrackData trackData)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardTrackData(trackData);
                });

                return _parent;
            }
        }

        public OfflineChargeBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditOfflineSaleReqType
                                {
                                    Block1 = new CreditOfflineSaleReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditOfflineSale
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditOfflineSale);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public OfflineChargeBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public OfflineChargeBuilder RequestMultiuseToken()
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardData.TokenRequest = booleanType.Y);
            return this;
        }

        public OfflineChargeBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public OfflineChargeBuilder WithGratuity(decimal gratuityAmount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfo = gratuityAmount;
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
                });

            return this;
        }

        public OfflineChargeBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public OfflineChargeBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }

        public OfflineChargeBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CardData.EncryptionData = HydrateEncryptionData(encryptionData));
            return this;
        }

        public OfflineChargeBuilder WithSurcharge(decimal amount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.SurchargeAmtInfo = amount;
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.SurchargeAmtInfoSpecified = true;
                });
            return this;
        }

        public OfflineChargeBuilder WithOfflineAuthCode(string offlineAuthCode)
        {
            BuilderActions.Add(n => ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.OfflineAuthCode = offlineAuthCode);
            return this;
        }

        public OfflineChargeBuilder WithCpcRequest(bool cpcRequest)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CPCReq = cpcRequest ? booleanType.Y : booleanType.N;
                    ((PosCreditOfflineSaleReqType) n.Transaction.Item).Block1.CPCReqSpecified = true;
                });
            return this;
        }
    }
}
