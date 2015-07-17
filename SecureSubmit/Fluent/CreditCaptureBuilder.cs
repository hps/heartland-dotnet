using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CreditCaptureBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsTransaction> {
        int? transactionId;
        decimal? amount;
        decimal? gratuity;
        string clientTransactionId;
        HpsDirectMarketData directMarketData;

        public CreditCaptureBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditCaptureBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public CreditCaptureBuilder WithGratuity(decimal? value) {
            this.gratuity = value;
            return this;
        }
        public CreditCaptureBuilder WithClientTransactionId(string value) {
            this.clientTransactionId = value;
            return this;
        }
        public CreditCaptureBuilder WithDirectMarketData(HpsDirectMarketData value) {
            this.directMarketData = value;
            return this;
        }

        public CreditCaptureBuilder(HpsFluentCreditService service) : base(service) { }

        public override HpsTransaction Execute() {
            base.Execute();

            var request = new PosCreditAddToBatchReqType {
                GatewayTxnId = transactionId.Value,
            };

            request.AmtSpecified = amount.HasValue;
            if(amount.HasValue) request.Amt = amount.Value;

            request.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (gratuity.HasValue) request.GratuityAmtInfo = gratuity.Value;

            if (directMarketData != null)
                request.DirectMktData = service.HydrateDirectMktData(directMarketData);

            var transaction = new PosRequestVer10Transaction{
                Item = request,
                ItemElementName = ItemChoiceType1.CreditAddToBatch
            };

            var response = service.SubmitTransaction(transaction);
            var trans = new HpsTransaction().FromResponse(response);
            trans.ResponseCode = "00";
            return trans;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "TransactionId is required.");
        }
    }
}
