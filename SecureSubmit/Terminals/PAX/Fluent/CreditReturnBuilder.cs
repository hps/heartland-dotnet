using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditReturnBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        private decimal? amount;
        HpsCreditCard card;
        string token;
        int? transactionId;
        HpsAddress address;
        HpsTransactionDetails details;
        bool allowDuplicates = false;
        string authCode;

        public CreditReturnBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public CreditReturnBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditReturnBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public CreditReturnBuilder WithToken(string value) {
            this.token = value;
            return this;
        }
        public CreditReturnBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditReturnBuilder WithAddress(HpsAddress value) {
            this.address = value;
            return this;
        }
        public CreditReturnBuilder WithDetails(HpsTransactionDetails value) {
            this.details = value;
            return this;
        }
        public CreditReturnBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }

        public CreditReturnBuilder WithAuthCode(string code) {
            this.authCode = code;
            return this;
        }

        public CreditReturnBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var amounts = new AmountRequest {
                TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric()
            };

            var account = new AccountRequest();
            if (card != null) {
                account.AccountNumber = card.Number;
                account.EXPD = "{0}{1}".FormatWith(card.ExpMonth, card.ExpYear);
            }
            if (allowDuplicates) account.DupOverrideFlag = "1";

            // Avs Sub Group
            var avs = new AvsRequest();
            if (address != null) {
                avs.ZipCode = address.Zip;
                avs.Address = address.Address;
            }

            // Trace Sub Group
            var trace = new TraceRequest {
                ReferenceNumber = referenceNumber.ToString()
            };
            if (details != null)
                trace.InvoiceNumber = details.InvoiceNumber;
            if (authCode != null)
                trace.AuthCode = authCode;

            var cashier = new CashierSubGroup();
            var commercial = new CommercialRequest();
            var ecom = new EcomSubGroup();

            // Additional Info sub group
            var additionalInfo = new ExtDataSubGroup();
            if (token != null)
                additionalInfo[EXT_DATA.TOKEN] = token;
            if (transactionId.HasValue)
                additionalInfo[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();

            return service.DoCredit(PAX_TXN_TYPE.RETURN, amounts, account, trace, avs, cashier, commercial, ecom, additionalInfo);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
            AddValidation(NoAuthCodeWithTransId, "Auth code is required for returns by transactionId");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (transactionId != null) count++;
            if (token != null) count++;

            return count <= 1;
        }

        private bool NoAuthCodeWithTransId() {
            if (transactionId != null)
                return authCode != null;
            return true;
        }
    }
}
