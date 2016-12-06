using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CheckVoidBuilder : HpsBuilderAbstract<HpsFluentCheckService, HpsCheckResponse> {
        long? transactionId;
        long? clientTransactionId;

        public CheckVoidBuilder withTransactionId(long? transactionId) {
            this.transactionId = transactionId;
            return this;
        }
        public CheckVoidBuilder withClientTransactionId(long? clientTransactionId) {
            this.clientTransactionId = clientTransactionId;
            return this;
        }

        public CheckVoidBuilder(HpsFluentCheckService service)
            : base(service) {
        }

        public override HpsCheckResponse Execute() {
            base.Execute();

            var block1 = new CheckVoidReqBlock1Type {
                GatewayTxnIdSpecified = transactionId.HasValue,
                ClientTxnIdSpecified = clientTransactionId.HasValue
            };

            if (block1.GatewayTxnIdSpecified)
                block1.GatewayTxnId = transactionId.Value;
            if (block1.ClientTxnIdSpecified)
                block1.ClientTxnId = clientTransactionId.Value;

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCheckVoidReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CheckVoid
            };

            return service.SubmitTransaction(transaction);
        }

        protected override void SetupValidations() {
            AddValidation(OnlyOneTransactionId, "You may only use one transaction id.");
        }

        private bool OnlyOneTransactionId() {
            int count = 0;
            if (this.transactionId != null) count++;
            if (this.clientTransactionId != null) count++;
            return count == 1;
        }
    }
}
