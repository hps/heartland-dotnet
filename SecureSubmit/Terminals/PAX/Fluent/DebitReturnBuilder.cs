using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX
{
    /*Does not match the existing DebitReturnBuilder*/
    /*are you referring to SecureSubmit\Fluent\DebitReturnBuilder.cs*/
    public class DebitReturnBuilder : HpsBuilderAbstract<PaxDevice, DebitResponse>
    {
        
        decimal? amount;
        private int? referenceNumber;
        private int? transactionId;

        public DebitReturnBuilder WithTransactionId(int? value)
        {
            this.transactionId = value;
            return this;
        }
        public DebitReturnBuilder WithReferenceNumber(int? referenceNumber)
        {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public DebitReturnBuilder WithAmount(decimal? value)
        {
            this.amount = value;
            return this;
        }
        public DebitReturnBuilder(PaxDevice device)
            : base(device)
        {
        }
        public override DebitResponse Execute()
        {
            base.Execute();

            var amounts = new AmountRequest
            {
                TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric()
            };

            var account = new AccountRequest();
            // Trace Sub Group
            var trace = new TraceRequest
            {
                ReferenceNumber = referenceNumber.ToString()
            };

            var cashier = new CashierSubGroup();

            // Additional Info sub group
            var extData = new ExtDataSubGroup();
            if (transactionId != null)
            {
            extData[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.ToString();
            }
            return service.DoDebit(PAX_TXN_TYPE.RETURN, amounts, account, trace, cashier, extData);
        }
        protected override void SetupValidations()
        {
            AddValidation(() => { return amount != null; }, "Amount is required.");
        }

    }
}
