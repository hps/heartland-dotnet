using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class BalanceInquiryBuilder : GatewayTransactionBuilder<BalanceInquiryBuilder, HpsAuthorization>
    {
        public class BalanceInquiryUsingBuilder
        {
            private readonly BalanceInquiryBuilder _parent;

            public BalanceInquiryUsingBuilder(BalanceInquiryBuilder parent)
            {
                _parent = parent;
            }

            public BalanceInquiryBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosPrePaidBalanceInquiryReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public BalanceInquiryBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosPrePaidBalanceInquiryReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }
        }

        public BalanceInquiryBuilder(IHpsServicesConfig config)
            : base(config)
        {
            BuilderActions.Add(n =>
            {
                n.Transaction = new PosRequestVer10Transaction
                {
                    Item = new PosPrePaidBalanceInquiryReqType
                    {
                        Block1 = new PrePaidBalanceInquiryReqBlock1Type
                        {
                            CardData = new CardDataType()
                        }
                    },
                    ItemElementName = ItemChoiceType1.PrePaidBalanceInquiry
                };
            });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.PrePaidBalanceInquiry);

            var result = HydrateAuthorization<HpsAuthorization>(response);
            HpsIssuerResponseValidation.CheckResponse(result.TransactionId, result.ResponseCode, result.ResponseText);

            return result;
        }

        public BalanceInquiryBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosPrePaidBalanceInquiryReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public BalanceInquiryBuilder RequestMultiuseToken(bool requestMultiuseToken = true)
        {
            BuilderActions.Add(n => ((PosPrePaidBalanceInquiryReqType)n.Transaction.Item).Block1.CardData.TokenRequest = requestMultiuseToken ? booleanType.Y : booleanType.N);
            return this;
        }

        public BalanceInquiryBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosPrePaidBalanceInquiryReqType)n.Transaction.Item).Block1.CardData.EncryptionData = HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
