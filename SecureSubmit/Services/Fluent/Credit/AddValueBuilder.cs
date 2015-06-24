using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class AddValueBuilder : GatewayTransactionBuilder<AddValueBuilder, HpsAuthorization>
    {
        public class AddValueUsingBuilder
        {
            private readonly AddValueBuilder _parent;

            public AddValueUsingBuilder(AddValueBuilder parent)
            {
                _parent = parent;
            }

            public AddValueBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(n => ((PosPrePaidAddValueReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardManualEntry(card));
                return _parent;
            }

            public AddValueBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosPrePaidAddValueReqType) n.Transaction.Item).Block1.CardData.Item = new CardDataTypeTokenData
                            {
                                TokenValue = token
                            };
                    });

                return _parent;
            }

            public AddValueBuilder WithTrackData(HpsTrackData trackData)
            {
                _parent.BuilderActions.Add(n => ((PosPrePaidAddValueReqType) n.Transaction.Item).Block1.CardData.Item = HydrateCardTrackData(trackData));
                return _parent;
            }
        }

        public AddValueBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosPrePaidAddValueReqType
                                {
                                    Block1 = new PrePaidAddValueReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            CardData = new CardDataType()
                                        }
                                },
                            ItemElementName = ItemChoiceType1.PrePaidAddValue
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.PrePaidAddValue);

            var result = HydrateAuthorization<HpsAuthorization>(response);
            HpsIssuerResponseValidation.CheckResponse(result.TransactionId, result.ResponseCode, result.ResponseText);

            return result;
        }

        public AddValueBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public AddValueBuilder RequestMultiuseToken(bool requestMultiuseToken = true)
        {
            BuilderActions.Add(n => ((PosCreditSaleReqType)n.Transaction.Item).Block1.CardData.TokenRequest = requestMultiuseToken ? booleanType.Y : booleanType.N);
            return this;
        }

        public AddValueBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosPrePaidAddValueReqType)n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosPrePaidAddValueReqType)n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public AddValueBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosPrePaidAddValueReqType)n.Transaction.Item).Block1.CardData.EncryptionData = HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
