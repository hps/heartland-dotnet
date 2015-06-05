using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Debit
{
    public class ReturnBuilder : GatewayTransactionBuilder<ReturnBuilder, HpsAuthorization>
    {
        public ReturnBuilder(IHpsServicesConfig config, decimal amount, string trackData, string pinBlock) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosDebitReturnReqType
                                {
                                    Block1 = new DebitReturnReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            TrackData = trackData,
                                            PinBlock = pinBlock
                                        }
                                },
                            ItemElementName = ItemChoiceType1.DebitReturn
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitReturn);

            var chargeResponse = (AuthRspStatusType) rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, chargeResponse.RspCode, chargeResponse.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }

        public ReturnBuilder WithTransactionId(int transactionId)
        {
            BuilderActions.Add(n =>
            {
                ((PosDebitReturnReqType) n.Transaction.Item).Block1.GatewayTxnId = transactionId;
                ((PosDebitReturnReqType) n.Transaction.Item).Block1.GatewayTxnIdSpecified = true;
            });
            return this;
        }

        public ReturnBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosDebitReturnReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public ReturnBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosDebitReturnReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosDebitReturnReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public ReturnBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosDebitReturnReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public ReturnBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosDebitReturnReqType) n.Transaction.Item).Block1.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
