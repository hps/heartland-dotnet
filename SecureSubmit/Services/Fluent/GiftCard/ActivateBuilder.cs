using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class ActivateBuilder : GatewayTransactionBuilder<ActivateBuilder, HpsGiftCardActivate>
    {
        public ActivateBuilder(IHpsServicesConfig config, decimal amount, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardActivateReqType
                            {
                                Block1 = new GiftCardActivateReqBlock1Type
                                {
                                    Amt = amount,
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardActivate
                        };
                });
        }

        public override HpsGiftCardActivate Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardActivate);

            var activationRsp = (PosGiftCardActivateRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                activationRsp.RspCode.ToString(CultureInfo.InvariantCulture), activationRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardTransactionResponse). */
            var activation = new HpsGiftCardActivate
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = activationRsp.AuthCode,
                BalanceAmount = activationRsp.BalanceAmt,
                PointsBalanceAmount = activationRsp.PointsBalanceAmt,
                Rewards = activationRsp.Rewards,
                Notes = activationRsp.Notes,
                ResponseCode = activationRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = activationRsp.RspText
            };

            return activation;
        }

        public ActivateBuilder WithCurrency(currencyType currency)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardActivateReqType) n.Transaction.Item).Block1.Currency = currency;
                ((PosGiftCardActivateReqType) n.Transaction.Item).Block1.CurrencySpecified = true;
            });

            return this;
        }
    }
}
