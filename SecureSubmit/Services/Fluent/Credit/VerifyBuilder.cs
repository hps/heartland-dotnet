using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class VerifyBuilder : GatewayTransactionBuilder<VerifyBuilder, HpsAccountVerify>
    {
        public class VerifyUsingBuilder
        {
            private readonly VerifyBuilder _parent;

            public VerifyUsingBuilder(VerifyBuilder parent)
            {
                _parent = parent;
            }

            public VerifyBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosCreditAccountVerifyReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public VerifyBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosCreditAccountVerifyReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                    {
                        TokenValue = token
                    };
                });

                return _parent;
            }

            public VerifyBuilder WithTrackData(HpsTrackData trackData)
            {
                _parent.BuilderActions.Add(n => ((PosCreditAccountVerifyReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardTrackData(trackData));
                return _parent;
            }
        }

        public VerifyBuilder(IHpsServicesConfig config)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditAccountVerifyReqType
                                {
                                    Block1 = new CreditAccountVerifyBlock1Type
                                        {
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.CreditAccountVerify
                        };
                });
        }

        public override HpsAccountVerify Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
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
                AuthorizedAmount = creditVerifyRsp.AuthAmt
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

        public VerifyBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditAccountVerifyReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public VerifyBuilder RequestMultiuseToken(bool requestMultiuseToken = true)
        {
            BuilderActions.Add(n => ((PosCreditAccountVerifyReqType)n.Transaction.Item).Block1.CardData.TokenRequest = requestMultiuseToken ? booleanType.Y : booleanType.N);
            return this;
        }

        public VerifyBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosCreditAccountVerifyReqType)n.Transaction.Item).Block1.CardData.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
