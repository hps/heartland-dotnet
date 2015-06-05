using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class RefundBuilder : GatewayTransactionBuilder<RefundBuilder, HpsRefund>
    {
        public class RefundUsingBuilder
        {
            private readonly RefundBuilder _parent;

            public RefundUsingBuilder(RefundBuilder parent)
            {
                _parent = parent;
            }

            public RefundBuilder WithTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditReturnReqType) n.Transaction.Item).Block1.GatewayTxnId = transactionId;
                        ((PosCreditReturnReqType) n.Transaction.Item).Block1.GatewayTxnIdSpecified = true;
                    });

                return _parent;
            }

            public RefundBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditReturnReqType)n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public RefundBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosCreditReturnReqType)n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                    {
                        TokenValue = token
                    };
                });

                return _parent;
            }
        }

        public RefundBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditReturnReqType
                                {
                                    Block1 = new CreditReturnReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditReturn
                        };
                });
        }

        public override HpsRefund Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
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

        public RefundBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditReturnReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public RefundBuilder RequestMultiuseToken()
        {
            BuilderActions.Add(n => ((PosCreditReturnReqType) n.Transaction.Item).Block1.CardData.TokenRequest = booleanType.Y);
            return this;
        }

        public RefundBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditReturnReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosCreditReturnReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public RefundBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditReturnReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public RefundBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditReturnReqType) n.Transaction.Item).Block1.DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }

        public RefundBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditReturnReqType) n.Transaction.Item).Block1.CardData.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
