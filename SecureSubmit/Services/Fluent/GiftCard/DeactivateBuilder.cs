using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class DeactivateBuilder : GatewayTransactionBuilder<DeactivateBuilder, HpsGiftCardDeactivate>
    {
        public DeactivateBuilder(IHpsServicesConfig config, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardDeactivateReqType
                            {
                                Block1 = new GiftCardDeactivateReqBlock1Type
                                {
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardDeactivate
                        };
                });
        }

        public override HpsGiftCardDeactivate Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardDeactivate);

            var deactivateRsp = (PosGiftCardDeactivateRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                deactivateRsp.RspCode.ToString(CultureInfo.InvariantCulture), deactivateRsp.RspText);

            var response = new HpsGiftCardDeactivate
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = deactivateRsp.AuthCode,
                RefundAmount = deactivateRsp.RefundAmt,
                PointsBalanceAmount = deactivateRsp.PointsBalanceAmt,
                ResponseCode = deactivateRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = deactivateRsp.RspText
            };

            return response;
        }
    }
}
