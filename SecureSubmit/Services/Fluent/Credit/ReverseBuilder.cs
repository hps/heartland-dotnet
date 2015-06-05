using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class ReverseBuilder : GatewayTransactionBuilder<ReverseBuilder, HpsReversal>
    {
        public class ReverseUsingBuilder
        {
            private readonly ReverseBuilder _parent;

            public ReverseUsingBuilder(ReverseBuilder parent)
            {
                _parent = parent;
            }

            public ReverseBuilder WithTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditReversalReqType) n.Transaction.Item).Block1.GatewayTxnId = transactionId;
                        ((PosCreditReversalReqType) n.Transaction.Item).Block1.GatewayTxnIdSpecified = true;
                    });

                return _parent;
            }

            public ReverseBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditReversalReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public ReverseBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosCreditReversalReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                    {
                        TokenValue = token
                    };
                });

                return _parent;
            }
        }

        public ReverseBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditReversalReqType
                                {
                                    Block1 = new CreditReversalReqBlock1Type
                                        {
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditReversal
                        };
                });
        }

        public override HpsReversal Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.CreditReversal);

            var result = (AuthRspStatusType)response.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(response.Header.GatewayTxnId, result.RspCode, result.RspText);

            return new HpsReversal
            {
                Header = HydrateTransactionHeader(response.Header),
                TransactionId = response.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(response.Header),
                AvsResultCode = result.AVSRsltCode,
                AvsResultText = result.AVSRsltText,
                CpcIndicator = result.CPCInd,
                CvvResultCode = result.CVVRsltCode,
                CvvResultText = result.CVVRsltText,
                ReferenceNumber = result.RefNbr,
                ResponseCode = result.RspCode,
                ResponseText = result.RspText
            };
        }

        public ReverseBuilder RequestMultiuseToken()
        {
            BuilderActions.Add(n => ((PosCreditReversalReqType) n.Transaction.Item).Block1.CardData.TokenRequest = booleanType.Y);
            return this;
        }

        public ReverseBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCreditReversalReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public ReverseBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditReversalReqType) n.Transaction.Item).Block1.CardData.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }

        public ReverseBuilder WithAuthorizedAmount(decimal authorizedAmount)
        {
            BuilderActions.Add(n =>
            {
                ((PosCreditReversalReqType) n.Transaction.Item).Block1.AuthAmt = authorizedAmount;
                ((PosCreditReversalReqType) n.Transaction.Item).Block1.AuthAmtSpecified = true;
            });
            return this;
        }
    }
}
