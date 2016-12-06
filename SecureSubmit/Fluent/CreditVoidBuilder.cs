using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditVoidBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsTransaction> {
        long? transactionId;
        long? clientTransactionId;

        public CreditVoidBuilder WithTransactionId(long? transactionId) {
            this.transactionId = transactionId;
            return this;
        }
        public CreditVoidBuilder WithClientTransactionId(long? clientTransactionId) {
            this.clientTransactionId = clientTransactionId;
            return this;
        }

        public CreditVoidBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsTransaction Execute() {
            base.Execute();

            var request = new PosCreditVoidReqType {
                GatewayTxnId = transactionId.Value
            };

            var transaction = new PosRequestVer10Transaction {
                Item = request,
                ItemElementName = ItemChoiceType1.CreditVoid
            };

            var response = service.SubmitTransaction(transaction, clientTransactionId);
            var trans = new HpsTransaction().FromResponse(response);
            trans.ResponseCode = "00";
            trans.ResponseText = string.Empty;
            return trans;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "TransactionID is required.");
        }
    }
}
