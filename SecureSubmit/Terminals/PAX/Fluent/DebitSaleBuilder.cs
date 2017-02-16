using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX
{
    /*Does not match the existing DebitChargeBuilder*/
    /*are you referring to SecureSubmit\Fluent\DebitChargeBuilder.cs*/
   // public class DebitSaleBuilder : HpsBuilderAbstract<PaxDevice, DebitResponse>


    public class DebitSaleBuilder : HpsBuilderAbstract<PaxDevice, DebitResponse>
    {
        bool allowDuplicates = false;
        decimal? amount;
        decimal? cashBack;
        HpsTransactionDetails details;
        private int referenceNumber;

        public DebitSaleBuilder WithReferenceNumber(int referenceNumber)
        {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public DebitSaleBuilder WithAmount(decimal? amount)
        {
            this.amount = amount;
            return this;
        }
        public DebitSaleBuilder WithCashBack(decimal? cashback)
        {
            this.cashBack = cashback;
            return this;
        }
        public DebitSaleBuilder WithDetails(HpsTransactionDetails details)
        {
            this.details = details;
            return this;
        }
        public DebitSaleBuilder WithAllowDuplicates(bool allowDuplicates)
        {
            this.allowDuplicates = allowDuplicates;
            return this;
        }

        public DebitSaleBuilder(PaxDevice device)
            : base(device)
        {
        }

        public override DebitResponse Execute()
        {
            base.Execute();
            
            var amounts = new AmountRequest
            {
                TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric(),
                CashBackAmount = "{0:c}".FormatWith(cashBack).ToNumeric()
            };

            var account = new AccountRequest();
            if (allowDuplicates) account.DupOverrideFlag = "1";
            // Trace Sub Group
            var trace = new TraceRequest
            {
                ReferenceNumber = referenceNumber.ToString()
            };

            if (details != null)
            {
                trace.InvoiceNumber = details.InvoiceNumber;
            }

            var cashier = new CashierSubGroup();

            // Additional Info sub group
            var extData = new ExtDataSubGroup();
            return service.DoDebit(PAX_TXN_TYPE.SALE_REDEEM, amounts, account, trace, cashier, extData);
        }
        /*Can remove this as it is in the method signature on the calling class. If they set it to zero every time that's their prerogative.*/
        /*I dont think i understand this comment do you mean remove protected override void SetupValidations() and the related call to it? or do you mean this line AddValidation(() => { return referenceNumber > 0; }, "Reference is required.");*/
        protected override void SetupValidations()
        {
            AddValidation(() => { return amount != null && amount > 0; }, "Amount is required.");
        }
        
    }
}
