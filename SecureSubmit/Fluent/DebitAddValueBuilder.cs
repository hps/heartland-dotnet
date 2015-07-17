using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class DebitAddValueBuilder {} /* : GatewayTransactionBuilder<DebitAddValueBuilder, HpsAuthorization>
    {
        public DebitAddValueBuilder(IHpsServicesConfig config, decimal amount) : base(config)
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
                                            Amt = amount
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

        public DebitAddValueBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public DebitAddValueBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public DebitAddValueBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public DebitAddValueBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }

        public DebitAddValueBuilder WithTrackData(HpsTrackData trackData)
        {
            BuilderActions.Add(n => ((PosDebitAddValueReqType)n.Transaction.Item).Block1.TrackData = trackData.Value);
            return this;
        }

        public DebitAddValueBuilder WithPinBlock(string pinBlock)
        {
            BuilderActions.Add(n =>
            {
                ((PosDebitSaleReqType)n.Transaction.Item).Block1.PinBlock = pinBlock;
            });
            return this;
        }
    } */
}
