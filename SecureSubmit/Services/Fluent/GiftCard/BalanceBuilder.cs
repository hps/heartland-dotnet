using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class BalanceBuilder : GatewayTransactionBuilder<BalanceBuilder, HpsGiftCardBalance>
    {
        public BalanceBuilder(IHpsServicesConfig config, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardBalanceReqType
                            {
                                Block1 = new GiftCardBalanceReqBlock1Type
                                {
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardBalance
                        };
                });
        }

        public override HpsGiftCardBalance Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardBalance);

            var balanceRsp = (PosGiftCardBalanceRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                balanceRsp.RspCode.ToString(CultureInfo.InvariantCulture), balanceRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardBalance
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = balanceRsp.AuthCode,
                BalanceAmount = balanceRsp.BalanceAmt,
                PointsBalanceAmount = balanceRsp.PointsBalanceAmt,
                Rewards = balanceRsp.Rewards,
                Notes = balanceRsp.Notes,
                ResponseCode = balanceRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = balanceRsp.RspText
            };

            return response;
        }
    }
}
