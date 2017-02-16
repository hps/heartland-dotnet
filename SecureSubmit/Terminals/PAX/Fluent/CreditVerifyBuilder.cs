using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditVerifyBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        private HpsCreditCard card;
        private HpsAddress address;
        private bool requestMultiUseToken = false;

        public CreditVerifyBuilder WithCard(HpsCreditCard card) {
            this.card = card;
            return this;
        }
        public CreditVerifyBuilder WithAddress(HpsAddress address) {
            this.address = address;
            return this;
        }
        public CreditVerifyBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditVerifyBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }

        public CreditVerifyBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var account = new AccountRequest();
            if (card != null) {
                account.AccountNumber = card.Number;
                account.EXPD = "{0}{1}".FormatWith(card.ExpMonth, card.ExpYear);
            }

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

            return service.DoCredit(
                requestMultiUseToken ? PAX_TXN_TYPE.TOKENIZE : PAX_TXN_TYPE.VERIFY,
                new AmountRequest(),
                account,
                trace,
                avs,
                new CashierSubGroup(),
                new CommercialRequest(),
                new EcomSubGroup(),
                new ExtDataSubGroup()
            );
        }
    }
}
