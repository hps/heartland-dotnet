using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditEditBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsTransaction> {
        int? transactionId;
        decimal? amount;
        decimal? gratuity;
        long? clientTransactionId;

        public CreditEditBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditEditBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public CreditEditBuilder WithGratuity(decimal? value) {
            this.gratuity = value;
            return this;
        }
        public CreditEditBuilder WithClientTransactionId(long? value) {
            this.clientTransactionId = value;
            return this;
        }

        public CreditEditBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsTransaction Execute() {
            base.Execute();

            var request = new PosCreditTxnEditReqType {
                GatewayTxnId = transactionId.Value
            };

            request.AmtSpecified = amount.HasValue;
            if (request.AmtSpecified)
                request.Amt = amount.Value;

            request.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (request.GratuityAmtInfoSpecified)
                request.GratuityAmtInfo = gratuity.Value;

            var transaction = new PosRequestVer10Transaction {
                Item = request,
                ItemElementName = ItemChoiceType1.CreditTxnEdit
            };

            var response = service.SubmitTransaction(transaction, clientTransactionId);
            HpsTransaction trans = new HpsTransaction().FromResponse(response);
            trans.ResponseCode = "00";
            trans.ResponseText = string.Empty;

            return trans;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "TransactionId is required.");
        }
    }
}
