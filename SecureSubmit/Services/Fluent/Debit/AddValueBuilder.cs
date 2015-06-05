using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Debit
{
    public class AddValueBuilder : GatewayTransactionBuilder<AddValueBuilder, HpsAuthorization>
    {
        public AddValueBuilder(IHpsServicesConfig config, decimal amount, string trackData, string pinBlock) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosDebitAddValueReqType
                                {
                                    Block1 = new DebitAddValueReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            TrackData = trackData,
                                            PinBlock = pinBlock
                                        }
                                },
                            ItemElementName = ItemChoiceType1.DebitAddValue
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitAddValue);

            var chargeResponse = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, chargeResponse.RspCode, chargeResponse.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }

        public AddValueBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public AddValueBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public AddValueBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public AddValueBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
