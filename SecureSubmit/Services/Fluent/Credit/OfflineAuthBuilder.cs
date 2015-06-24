using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class OfflineAuthBuilder : GatewayTransactionBuilder<OfflineAuthBuilder, HpsTransaction>
    {
        public class OfflineAuthPaymentTypeBuilder
        {
            private readonly OfflineAuthBuilder _parent;

            public OfflineAuthPaymentTypeBuilder(OfflineAuthBuilder parent)
            {
                _parent = parent;
            }

            public OfflineAuthBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditOfflineAuthReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public OfflineAuthBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }

            public OfflineAuthBuilder WithTrackData(HpsTrackData trackData)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CardData.Item = HydrateCardTrackData(trackData);
                });

                return _parent;
            }
        }

        public OfflineAuthBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditOfflineAuthReqType
                                {
                                    Block1 = new CreditOfflineAuthReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditOfflineAuth
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditOfflineAuth);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public OfflineAuthBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public OfflineAuthBuilder RequestMultiuseToken()
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CardData.TokenRequest = booleanType.Y);
            return this;
        }

        public OfflineAuthBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public OfflineAuthBuilder WithGratuity(decimal gratuityAmount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.GratuityAmtInfo = gratuityAmount;
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
                });

            return this;
        }

        public OfflineAuthBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public OfflineAuthBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }

        public OfflineAuthBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CardData.EncryptionData = HydrateEncryptionData(encryptionData));
            return this;
        }

        public OfflineAuthBuilder WithSurcharge(decimal amount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.SurchargeAmtInfo = amount;
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.SurchargeAmtInfoSpecified = true;
                });
            return this;
        }

        public OfflineAuthBuilder WithOfflineAuthCode(string offlineAuthCode)
        {
            BuilderActions.Add(n => ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.OfflineAuthCode = offlineAuthCode);
            return this;
        }

        public OfflineAuthBuilder WithCpcRequest(bool cpcRequest)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CPCReq = cpcRequest ? booleanType.Y : booleanType.N;
                    ((PosCreditOfflineAuthReqType)n.Transaction.Item).Block1.CPCReqSpecified = true;
                });
            return this;
        }
    }
}
