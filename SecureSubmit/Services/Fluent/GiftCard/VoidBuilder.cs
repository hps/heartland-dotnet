using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class VoidBuilder : GatewayTransactionBuilder<VoidBuilder, HpsGiftCardVoid>
    {
        public VoidBuilder(IHpsServicesConfig config, int transactionId)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardVoidReqType
                            {
                                Block1 = new GiftCardVoidReqBlock1Type
                                {
                                    GatewayTxnId = transactionId
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardVoid
                        };
                });
        }

        public override HpsGiftCardVoid Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardVoid);

            var voidRsp = (PosGiftCardVoidRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                voidRsp.RspCode.ToString(CultureInfo.InvariantCulture), voidRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardVoid
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = voidRsp.AuthCode,
                BalanceAmount = voidRsp.BalanceAmt,
                PointsBalanceAmount = voidRsp.PointsBalanceAmt,
                Notes = voidRsp.Notes,
                ResponseCode = voidRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = voidRsp.RspText
            };

            return response;
        }
    }
}
