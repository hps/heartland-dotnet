using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class AddValueBuilder : GatewayTransactionBuilder<AddValueBuilder, HpsGiftCardAddValue>
    {
        public AddValueBuilder(IHpsServicesConfig config, decimal amount, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardAddValueReqType
                            {
                                Block1 = new GiftCardAddValueReqBlock1Type
                                {
                                    Amt = amount,
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardAddValue
                        };
                });
        }

        public override HpsGiftCardAddValue Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardAddValue);

            var addValueRsp = (PosGiftCardAddValueRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                addValueRsp.RspCode.ToString(CultureInfo.InvariantCulture), addValueRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var addValue = new HpsGiftCardAddValue
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = addValueRsp.AuthCode,
                BalanceAmount = addValueRsp.BalanceAmt,
                PointsBalanceAmount = addValueRsp.PointsBalanceAmt,
                Rewards = addValueRsp.Rewards,
                Notes = addValueRsp.Notes,
                ResponseCode = addValueRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = addValueRsp.RspText
            };

            return addValue;
        }

        public AddValueBuilder WithCurrency(currencyType currency)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardAddValueReqType)n.Transaction.Item).Block1.Currency = currency;
                ((PosGiftCardAddValueReqType)n.Transaction.Item).Block1.CurrencySpecified = true;
            });

            return this;
        }
    }
}
