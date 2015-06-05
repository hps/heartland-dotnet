using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Debit
{
    public class ChargeBuilder : GatewayTransactionBuilder<ChargeBuilder, HpsAuthorization>
    {
        public ChargeBuilder(IHpsServicesConfig config, decimal amount, string trackData, string pinBlock) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosDebitSaleReqType
                                {
                                    Block1 = new DebitSaleReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            TrackData = trackData,
                                            PinBlock = pinBlock
                                        }
                                },
                            ItemElementName = ItemChoiceType1.DebitSale
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitSale);

            var chargeResponse = (AuthRspStatusType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, chargeResponse.RspCode, chargeResponse.RspText);

            return HydrateAuthorization<HpsAuthorization>(rsp);
        }

        public ChargeBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosDebitSaleReqType) n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public ChargeBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosDebitSaleReqType) n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosDebitSaleReqType) n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public ChargeBuilder AllowPartialAuth(bool allowPartialAuth)
        {
            BuilderActions.Add(n =>
                {
                    ((PosDebitSaleReqType) n.Transaction.Item).Block1.AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N;
                    ((PosDebitSaleReqType) n.Transaction.Item).Block1.AllowPartialAuthSpecified = true;
                });

            return this;
        }

        public ChargeBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosDebitSaleReqType) n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public ChargeBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosDebitSaleReqType) n.Transaction.Item).Block1.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }

        public ChargeBuilder WithCashBackAmount(decimal cashBackAmount)
        {
            BuilderActions.Add(n =>
            {
                ((PosDebitSaleReqType) n.Transaction.Item).Block1.CashbackAmtInfo = cashBackAmount;
                ((PosDebitSaleReqType)n.Transaction.Item).Block1.CashbackAmtInfoSpecified = true;
            });

            return this;
        }
    }
}
