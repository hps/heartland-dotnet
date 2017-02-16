using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX
{
    public class GiftAddValueBuilder : HpsBuilderAbstract<PaxDevice, GiftResponse>
    {
        private decimal? amount;
        private HpsGiftCard card;
        private int referenceNumber;
        private currencyType? currency = currencyType.USD;

        public GiftAddValueBuilder WithReferenceNumber(int referenceNumber)
        {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public GiftAddValueBuilder WithAmount(decimal? value)
        {
            this.amount = value;
            return this;
        }
        public GiftAddValueBuilder WithCard(HpsGiftCard value)
        {
            this.card = value;
            return this;
        }
        public GiftAddValueBuilder WithCurrency(currencyType? value)
        {
            this.currency = value;
            return this;
        }

        public GiftAddValueBuilder(PaxDevice device)
            : base(device)
        {
        }

        public override GiftResponse Execute()
        {
            base.Execute();

            var amounts = new AmountRequest { TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric() };
            var account = new AccountRequest { AccountNumber = card != null ? card.Value : null };
            var trace = new TraceRequest { ReferenceNumber = referenceNumber.ToString() };
            var cashier = new CashierSubGroup();
            var extData = new ExtDataSubGroup();

            // TODO : Change to TransactionType RETURN when terminal adds support for it
            var messageId = currency == currencyType.USD ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
            return service.DoGift(messageId, PAX_TXN_TYPE.ADD, amounts, account, trace, cashier, extData);
        }

        protected override void SetupValidations()
        {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return currency.HasValue; }, "Currency is required.");
        }
    }
}
