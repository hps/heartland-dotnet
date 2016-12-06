using System;
using SecureSubmit.Entities;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Fluent.Services;

namespace SecureSubmit.Fluent {
    public class CreditGetBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsReportTransactionDetails> {
        long? transactionId;

        public CreditGetBuilder WithTransactionId(long? transactionId) {
            this.transactionId = transactionId;
            return this;
        }

        public CreditGetBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsReportTransactionDetails Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosReportTxnDetailReqType {
                    TxnId = transactionId.Value
                },
                ItemElementName = ItemChoiceType1.ReportTxnDetail
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsReportTransactionDetails().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "TransactionId is required.");
        }
    }
}
