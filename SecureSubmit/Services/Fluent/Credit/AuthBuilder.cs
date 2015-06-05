using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class AuthBuilder : GatewayTransactionBuilder<AuthBuilder, HpsAuthorization>
    {
        public class AuthPaymentTypeBuilder
        {
            private readonly AuthBuilder _parent;

            public AuthPaymentTypeBuilder(AuthBuilder parent)
            {
                _parent = parent;
            }

            public AuthBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public AuthBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditAuthReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }
        }

        public AuthBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditAuthReqType
                                {
                                    Block1 = new CreditAuthReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditAuth
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.CreditAuth);

            var result = HydrateAuthorization<HpsAuthorization>(response);
            HpsIssuerResponseValidation.CheckResponse(result.TransactionId, result.ResponseCode, result.ResponseText);

            return result;
        }

        public AuthBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public AuthBuilder RequestMultiuseToken(bool requestMultiuseToken = true)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType)n.Transaction.Item).Block1.CardData.TokenRequest = requestMultiuseToken ? booleanType.Y : booleanType.N);
            return this;
        }

        public AuthBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public AuthBuilder WithDescriptor(string descriptor)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.TxnDescriptor = descriptor);
            return this;
        }

        public AuthBuilder AllowPartialAuth(bool allowPartialAuth)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N;
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.AllowPartialAuthSpecified = true;
                });

            return this;
        }

        public AuthBuilder WithGratuity(decimal gratuityAmount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.GratuityAmtInfo = gratuityAmount;
                    ((PosCreditAuthReqType) n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
                });

            return this;
        }

        public AuthBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public AuthBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }

        public AuthBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditAuthReqType) n.Transaction.Item).Block1.CardData.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
