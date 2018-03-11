using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditVoidBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        private int? transactionId;

        public CreditVoidBuilder WithTransactionId(int? transactionId) {
            this.transactionId = transactionId;
            return this;
        }
        public CreditVoidBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }

        public CreditVoidBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var extData = new ExtDataSubGroup();
            if (transactionId.HasValue)
            {
                extData[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();
            }

            return service.DoCredit(PAX_TXN_TYPE.VOID,
                new AmountRequest(),
                new AccountRequest(),
                new TraceRequest { ReferenceNumber = referenceNumber.ToString() },
                new AvsRequest(),
                new CashierSubGroup(),
                new CommercialRequest(),
                new EcomSubGroup(),
                extData
            );
        }
    }
}
