using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditAdjustBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        decimal? gratuity;
        int? transactionId;

        public CreditAdjustBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }

        public CreditAdjustBuilder WithGratuity(decimal? value)
        {
            this.gratuity = value;
            return this;
        }

        public CreditAdjustBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }

        public CreditAdjustBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var amounts = new AmountRequest();
            if (gratuity.HasValue)
                amounts.TransactionAmount = "{0:c}".FormatWith(gratuity).ToNumeric();

            var trace = new TraceRequest {
                ReferenceNumber = referenceNumber.ToString(),
                TransactionNumber = referenceNumber.ToString()
            };

            var extData = new ExtDataSubGroup();
            if (transactionId.HasValue)
                extData[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();

            var response = service.DoCredit(
                PAX_TXN_TYPE.ADJUST,
                amounts,
                new AccountRequest(),
                trace,
                new AvsRequest(),
                new CashierSubGroup(),
                new CommercialRequest(),
                new EcomSubGroup(),
                extData
            );

            return response;
        }
    }
}
