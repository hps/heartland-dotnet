using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class ReplaceBuilder : GatewayTransactionBuilder<ReplaceBuilder, HpsGiftCardReplace>
    {
        public ReplaceBuilder(IHpsServicesConfig config, HpsGiftCard oldGiftCard, HpsGiftCard newGiftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardReplaceReqType
                            {
                                Block1 = new GiftCardReplaceReqBlock1Type
                                {
                                    OldCardData = HydrateGiftCardData(oldGiftCard),
                                    NewCardData = HydrateGiftCardData(newGiftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardReplace
                        };
                });
        }

        public override HpsGiftCardReplace Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReplace);

            var replaceRsp = (PosGiftCardReplaceRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                replaceRsp.RspCode.ToString(CultureInfo.InvariantCulture), replaceRsp.RspText);

            var response = new HpsGiftCardReplace
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = replaceRsp.AuthCode,
                BalanceAmount = replaceRsp.BalanceAmt,
                PointsBalanceAmount = replaceRsp.PointsBalanceAmt,
                ResponseCode = replaceRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = replaceRsp.RspText
            };

            return response;
        }
    }
}
