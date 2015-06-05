using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class ReverseBuilder : GatewayTransactionBuilder<ReverseBuilder, HpsGiftCardReversal>
    {
        public class ReverseUsingBuilder
        {
            private readonly ReverseBuilder _parent;

            public ReverseUsingBuilder(ReverseBuilder parent)
            {
                _parent = parent;
            }

            public ReverseBuilder UsingTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosGiftCardReversalReqType)n.Transaction.Item).Block1.GatewayTxnId = transactionId;
                        ((PosGiftCardReversalReqType)n.Transaction.Item).Block1.GatewayTxnIdSpecified = true;
                    });

                return _parent;
            }

            public ReverseBuilder UsingCard(HpsGiftCard giftCard)
            {
                _parent.BuilderActions.Add(n => ((PosGiftCardReversalReqType)n.Transaction.Item).Block1.CardData = HydrateGiftCardData(giftCard));
                return _parent;
            }
        }

        public ReverseBuilder(IHpsServicesConfig config, decimal amount)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardReversalReqType
                            {
                                Block1 = new GiftCardReversalReqBlock1Type
                                {
                                    Amt = amount
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardReversal
                        };
                });
        }

        public override HpsGiftCardReversal Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardReversal);

            var reversalRsp = (PosGiftCardReversalRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                reversalRsp.RspCode.ToString(CultureInfo.InvariantCulture), reversalRsp.RspText);

            /* Start to fill out a new transaction response. */
            var reversal = new HpsGiftCardReversal
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = reversalRsp.AuthCode,
                BalanceAmount = reversalRsp.BalanceAmt,
                ResponseCode = reversalRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = reversalRsp.RspText
            };

            return reversal;
        }

        public ReverseBuilder UsingClientTransactionId(int clientTransactionId)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardReversalReqType) n.Transaction.Item).Block1.ClientTxnId = clientTransactionId;
                ((PosGiftCardReversalReqType)n.Transaction.Item).Block1.ClientTxnIdSpecified = true;
            });

            return this;
        }
    }
}
