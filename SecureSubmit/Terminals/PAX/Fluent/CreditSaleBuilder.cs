using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditSaleBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private decimal? amount;
        private HpsCreditCard card;
        private string token;
        private HpsAddress address;
        private bool requestMultiUseToken = false;
        private HpsTransactionDetails details;
        private bool allowDuplicates = false;
        private decimal? gratuity;
        private int referenceNumber;
        private bool signatureCapture;

        public CreditSaleBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }
        public CreditSaleBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditSaleBuilder WithCard(HpsCreditCard card) {
            this.card = card;
            return this;
        }
        public CreditSaleBuilder WithToken(string token) {
            this.token = token;
            return this;
        }
        public CreditSaleBuilder WithAddress(HpsAddress address) {
            this.address = address;
            return this;
        }
        public CreditSaleBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditSaleBuilder WithDetails(HpsTransactionDetails details) {
            this.details = details;
            return this;
        }
        public CreditSaleBuilder WithAllowDuplicates(bool allowDuplicates) {
            this.allowDuplicates = allowDuplicates;
            return this;
        }
        public CreditSaleBuilder WithGratuity(decimal? gratuity) {
            this.gratuity = gratuity;
            return this;
        }
        public CreditSaleBuilder WithSignatureCapture(bool signatureCapture) {
            this.signatureCapture = signatureCapture;
            return this;
        }

        public CreditSaleBuilder(PaxDevice device)
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

            var cashier = new CashierSubGroup();
            var commercial = new CommercialRequest();
            var ecom = new EcomSubGroup();
            
            // Additional Info sub group
            var extData = new ExtDataSubGroup();
            if (requestMultiUseToken)
                extData[EXT_DATA.TOKEN_REQUEST] = "1";

            if (token != null)
                extData[EXT_DATA.TOKEN] = token;

            if (signatureCapture)
                extData[EXT_DATA.SIGNATURE_CAPTURE] = "1";

            return service.DoCredit(PAX_TXN_TYPE.SALE_REDEEM, amounts, account, trace, avs, cashier, commercial, ecom, extData);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount != null; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (token != null) count++;

            return count <= 1;
        }
    }
}
