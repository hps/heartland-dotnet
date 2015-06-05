using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class ChargeBuilder : GatewayTransactionBuilder<ChargeBuilder, HpsCharge>
    {
        public class ChargePaymentTypeBuilder
        {
            private readonly ChargeBuilder _parent;

            public ChargePaymentTypeBuilder(ChargeBuilder parent)
            {
                _parent = parent;
            }

            public ChargeBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public ChargeBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditSaleReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }
        }

        public ChargeBuilder(IHpsServicesConfig config, decimal amount) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditSaleReqType
                                {
                                    Block1 = new CreditSaleReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditSale
                        };
                });
        }

        public override HpsCharge Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.CreditSale);

            var result = HydrateAuthorization<HpsCharge>(response);
            HpsIssuerResponseValidation.CheckResponse(result.TransactionId, result.ResponseCode, result.ResponseText);

            return result;
        }

        public ChargeBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public ChargeBuilder RequestMultiuseToken(bool requestMultiuseToken = true)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType)n.Transaction.Item).Block1.CardData.TokenRequest = requestMultiuseToken ? booleanType.Y : booleanType.N);
            return this;
        }

        public ChargeBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public ChargeBuilder WithDescriptor(string descriptor)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.TxnDescriptor = descriptor);
            return this;
        }

        public ChargeBuilder AllowPartialAuth(bool allowPartialAuth = true)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N;
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.AllowPartialAuthSpecified = true;
                });

            return this;
        }

        public ChargeBuilder WithGratuity(decimal gratuityAmount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfo = gratuityAmount;
                    ((PosCreditSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
                });

            return this;
        }

        public ChargeBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public ChargeBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }

        public ChargeBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.CardData.EncryptionData = HydrateEncryptionData(encryptionData));
            return this;
        }

        public ChargeBuilder WithCpcReq(bool cpcRequested = true)
        {
            BuilderActions.Add(n =>
            {
                ((PosCreditSaleReqType)n.Transaction.Item).Block1.CPCReq = cpcRequested ? booleanType.Y : booleanType.N;
                ((PosCreditSaleReqType)n.Transaction.Item).Block1.CPCReqSpecified = true;
            });

            return this;
        }
    }
}
