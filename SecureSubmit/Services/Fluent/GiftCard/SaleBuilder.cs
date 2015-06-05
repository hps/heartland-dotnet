using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.GiftCard
{
    public class SaleBuilder : GatewayTransactionBuilder<SaleBuilder, HpsGiftCardSale>
    {
        public SaleBuilder(IHpsServicesConfig config, decimal amount, HpsGiftCard giftCard)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosGiftCardSaleReqType
                            {
                                Block1 = new GiftCardSaleReqBlock1Type
                                {
                                    Amt = amount,
                                    CardData = HydrateGiftCardData(giftCard)
                                }
                            },
                            ItemElementName = ItemChoiceType1.GiftCardSale
                        };
                });
        }

        public override HpsGiftCardSale Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.GiftCardSale);

            var saleRsp = (PosGiftCardSaleRspType)rsp.Transaction.Item;
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId,
                saleRsp.RspCode.ToString(CultureInfo.InvariantCulture), saleRsp.RspText);

            /* Start to fill out a new transaction response. */
            var response = new HpsGiftCardSale
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = saleRsp.AuthCode,
                BalanceAmount = saleRsp.BalanceAmt,
                SplitTenderCardAmount = saleRsp.SplitTenderCardAmt,
                SplitTenderBalanceDue = saleRsp.SplitTenderBalanceDueAmt,
                PointsBalanceAmount = saleRsp.PointsBalanceAmt,
                ResponseCode = saleRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = saleRsp.RspText
            };

            return response;
        }

        public SaleBuilder WithCurrency(currencyType currency)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.Currency = currency;
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.CurrencySpecified = true;
            });

            return this;
        }

        public SaleBuilder WithGratuity(decimal gratuity)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfo = gratuity;
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.GratuityAmtInfoSpecified = true;
            });

            return this;
        }

        public SaleBuilder WithTax(decimal tax)
        {
            BuilderActions.Add(n =>
            {
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.TaxAmtInfo = tax;
                ((PosGiftCardSaleReqType) n.Transaction.Item).Block1.TaxAmtInfoSpecified = true;
            });

            return this;
        }
    }
}
