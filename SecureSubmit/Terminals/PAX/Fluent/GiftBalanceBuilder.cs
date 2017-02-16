using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX
{
    public class GiftBalanceBuilder : HpsBuilderAbstract<PaxDevice, GiftResponse>
    {
        private HpsGiftCard card;
        private currencyType? currency = currencyType.USD;
        private int referenceNumber;

        public GiftBalanceBuilder WithReferenceNumber(int referenceNumber)
        {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public GiftBalanceBuilder WithCard(HpsGiftCard value)
        {
            this.card = value;
            return this;
        }
        public GiftBalanceBuilder WithCurrency(currencyType? value)
        {
            this.currency = value;
            return this;
        }

        public GiftBalanceBuilder(PaxDevice device)
            : base(device)
        {
        }

        public override GiftResponse Execute()
        {
            base.Execute();

            var account = new AccountRequest { AccountNumber = card != null ? card.Value : null };
            var trace = new TraceRequest { ReferenceNumber = referenceNumber.ToString() };
            var cashier = new CashierSubGroup();
            var extData = new ExtDataSubGroup();

            var messageId = currency == currencyType.USD ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
            return service.DoGift(messageId, PAX_TXN_TYPE.BALANCE,new AmountRequest(), account, trace, cashier, extData);
        }

        protected override void SetupValidations()
        {
            AddValidation(() => { return currency.HasValue; }, "Currency is required.");
        }
    }
}
