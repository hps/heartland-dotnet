using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class RewardBuilder : GatewayTransactionBuilder<RewardBuilder, HpsGiftCardReward>
    {
        public RewardBuilder(IHpsServicesConfig config, decimal amount, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardRewardReqType
                            {
                                Block1 = new GiftCardRewardReqBlock1Type
                                {
                                    Amt = amount,
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardReward
                        };
                });
        }

        public override HpsGiftCardReward Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReward);

            var rewardRsp = (PosGiftCardRewardRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                rewardRsp.RspCode.ToString(CultureInfo.InvariantCulture), rewardRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardReward
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = rewardRsp.AuthCode,
                BalanceAmount = rewardRsp.BalanceAmt,
                PointsBalanceAmount = rewardRsp.PointsBalanceAmt,
                ResponseCode = rewardRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = rewardRsp.RspText
            };

            return response;
        }

        public RewardBuilder WithCurrency(currencyType currency)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.Currency = currency;
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.CurrencySpecified = true;
            });

            return this;
        }

        public RewardBuilder WithGratuity(decimal gratuity)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.GratuityAmtInfo = gratuity;
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
            });

            return this;
        }

        public RewardBuilder WithTax(decimal tax)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.TaxAmtInfo = tax;
                ((PosGiftCardRewardReqType)n.Transaction.Item).Block1.TaxAmtInfoSpecified = true;
            });

            return this;
        }
    }
}
