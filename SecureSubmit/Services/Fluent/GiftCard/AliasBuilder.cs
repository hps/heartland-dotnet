using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class AliasBuilder : GatewayTransactionBuilder<AliasBuilder, HpsGiftCardAlias>
    {
        public AliasBuilder(IHpsServicesConfig config, HpsGiftCardAliasAction action, string alias)
            : base(config)
        {
            var gatewayAction = GiftCardAliasReqBlock1TypeAction.ADD;
            switch (action)
            {
                case HpsGiftCardAliasAction.Create: gatewayAction = GiftCardAliasReqBlock1TypeAction.CREATE; break;
                case HpsGiftCardAliasAction.Delete: gatewayAction = GiftCardAliasReqBlock1TypeAction.DELETE; break;
            }

            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardAliasReqType
                            {
                                Block1 = new GiftCardAliasReqBlock1Type
                                {
                                    Action = gatewayAction,
                                    Alias = alias
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardAlias
                        };
                });
        }

        public override HpsGiftCardAlias Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardAlias);

            var aliasRsp = (PosGiftCardAliasRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                aliasRsp.RspCode.ToString(CultureInfo.InvariantCulture), aliasRsp.RspText);

            /* Start to fill out a new transaction response (HpsGiftCardAddValue). */
            var response = new HpsGiftCardAlias
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                GiftCard = new HpsGiftCard
                {
                    Number = aliasRsp.CardData.CardNbr
                },
                ResponseCode = aliasRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = aliasRsp.RspText
            };

            return response;
        }

        public AliasBuilder WithGiftCard(HpsGiftCard giftCard)
        {
            BuilderActions.Add(n => ((PosGiftCardAliasReqType)n.Transaction.Item).Block1.CardData = HydrateGiftCardData(giftCard));
            return this;
        }
    }
}
