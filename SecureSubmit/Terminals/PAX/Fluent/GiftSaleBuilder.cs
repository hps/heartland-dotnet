using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class GiftSaleBuilder : HpsBuilderAbstract<PaxDevice, GiftResponse> {
        private HpsGiftCard card;
        private decimal? amount;
        private decimal? gratuity;
        private int referenceNumber;
        private currencyType? currency = currencyType.USD;
        private HpsTransactionDetails details;

        public GiftSaleBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public GiftSaleBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public GiftSaleBuilder WithGratuity(decimal? value)
        {
            this.gratuity = value;
            return this;
        }
        public GiftSaleBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }
        public GiftSaleBuilder WithCurrency(currencyType? value) {
            this.currency = value;
            return this;
        } 
        public GiftSaleBuilder WithDetails(HpsTransactionDetails details)
        {
            this.details = details;
            return this;
        }

        public GiftSaleBuilder(PaxDevice device)
            : base(device) {
        }

        public override GiftResponse Execute() {
            base.Execute();

            var amounts = new AmountRequest {
                TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric(),
                TipAmount = "{0:c}".FormatWith(gratuity).ToNumeric(),
            };

            var account = new AccountRequest { AccountNumber = card != null ? card.Value : null };
            var trace = new TraceRequest
            {
                ReferenceNumber = referenceNumber.ToString(),
                InvoiceNumber = details != null ? details.InvoiceNumber : null
            };

            var cashier = new CashierSubGroup();
            var extData = new ExtDataSubGroup();

            var messageId = currency == currencyType.USD ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
            return service.DoGift(messageId, PAX_TXN_TYPE.SALE_REDEEM, amounts, account, trace, cashier, extData);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return currency.HasValue; }, "Currency is required.");
        }
    }
}
