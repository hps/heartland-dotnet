using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX
{
    public class GiftVoidBuilder : HpsBuilderAbstract<PaxDevice, GiftResponse>
    {
        private int? transactionId;
        private int referenceNumber;
        private currencyType? currency = currencyType.USD;

        public GiftVoidBuilder WithReferenceNumber(int referenceNumber)
        {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public GiftVoidBuilder WithTransactionId(int? value)
        {
            this.transactionId = value;
            return this;
        }
        public GiftVoidBuilder WithCurrency(currencyType? value)
        {
            this.currency = value;
            return this;
        }

        public GiftVoidBuilder(PaxDevice device)
            : base(device)
        {
        }

        public override GiftResponse Execute()
        {
            base.Execute();

            var amounts = new AmountRequest();

            var account = new AccountRequest();

            var trace = new TraceRequest { ReferenceNumber = referenceNumber.ToString() };

            var extData = new ExtDataSubGroup();
            extData[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();

            var messageId = currency == currencyType.USD ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
            return service.DoGift(messageId, PAX_TXN_TYPE.VOID, amounts, account, trace, new CashierSubGroup(), extData);
        }

        protected override void SetupValidations()
        {
            AddValidation(() => { return transactionId.HasValue && transactionId > 0; }, "Transaction Id is required.");
            AddValidation(() => { return currency != null; }, "Currency is required.");
        }
    }
}
