using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Abstractions;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditAuthBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        private decimal? amount;
        private HpsCreditCard card;
        private string token;
        private HpsAddress address;
        private bool requestMultiUseToken = false;
        private HpsTransactionDetails details;
        private bool allowDuplicates = false;
        private decimal? gratuity;
        private int? transactionId;
        private string authCode;
        private bool signatureCapture;

        public CreditAuthBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public CreditAuthBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditAuthBuilder WithCard(HpsCreditCard card) {
            this.card = card;
            return this;
        }
        public CreditAuthBuilder WithToken(string token) {
            this.token = token;
            return this;
        }
        public CreditAuthBuilder WithAddress(HpsAddress address) {
            this.address = address;
            return this;
        }
        public CreditAuthBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditAuthBuilder WithDetails(HpsTransactionDetails details) {
            this.details = details;
            return this;
        }
        public CreditAuthBuilder WithAllowDuplicates(bool allowDuplicates) {
            this.allowDuplicates = allowDuplicates;
            return this;
        }
        public CreditAuthBuilder WithGratuity(decimal? gratuity) {
            this.gratuity = gratuity;
            return this;
        }
        public CreditAuthBuilder WithAuthCode(string value) {
            this.authCode = value;
            return this;
        }
        public CreditAuthBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditAuthBuilder WithSignatureCapture(bool signatureCapture) {
            this.signatureCapture = signatureCapture;
            return this;
        }

        public CreditAuthBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var amounts = new AmountRequest {
                TransactionAmount = "{0:c}".FormatWith(amount).ToNumeric(),
                TipAmount = "{0:c}".FormatWith(gratuity).ToNumeric()
            };

            var account = new AccountRequest();
            if (card != null) {
                account.AccountNumber = card.Number;
                account.EXPD = "{0}{1}".FormatWith(card.ExpMonth, card.ExpYear);
                account.CvvCode = card.Cvv;
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
            if (details != null) {
                trace.InvoiceNumber = details.InvoiceNumber;
            }
            if (!string.IsNullOrEmpty(authCode))
                trace.AuthCode = authCode;


            var cashier = new CashierSubGroup();
            var commercial = new CommercialRequest();
            var ecom = new EcomSubGroup();

            // Additional Info sub group
            var additionalInfo = new ExtDataSubGroup();
            if (requestMultiUseToken)
                additionalInfo[EXT_DATA.TOKEN_REQUEST] = "1";
            if (transactionId.HasValue)
                additionalInfo[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();
            if (token != null)
                additionalInfo[EXT_DATA.TOKEN] = token;
            if (signatureCapture)
                additionalInfo[EXT_DATA.SIGNATURE_CAPTURE] = "1";

            return service.DoCredit(PAX_TXN_TYPE.AUTH, amounts, account, trace, avs, cashier, commercial, ecom, additionalInfo);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount != null; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
            AddValidation(() => {
                if (transactionId.HasValue)
                    return !string.IsNullOrEmpty(authCode);
                return true;
            }, "AuthCode is required when using a transaction id.");
        }
        
        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (transactionId.HasValue) count++;
            if (token != null) count++;

            return count <= 1;
        }
    }
}
