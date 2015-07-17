using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditListBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsReportTransactionSummary[]> {
        DateTime utcStartDateTime;
        DateTime utcEndDateTime;
        HpsTransactionType filterBy;

        public CreditListBuilder WithUtcStartDateTime(DateTime startDateTime) {
            this.utcStartDateTime = startDateTime;
            return this;
        }
        public CreditListBuilder WithUtcEndDateTime(DateTime endDateTime) {
            this.utcEndDateTime = endDateTime;
            return this;
        }
        public CreditListBuilder WithFilterBy(HpsTransactionType filterBy) {
            this.filterBy = filterBy;
            return this;
        }

        public CreditListBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsReportTransactionSummary[] Execute() {
            base.Execute();

            HpsInputValidation.CheckDateNotFuture(utcStartDateTime);
            HpsInputValidation.CheckDateNotFuture(utcEndDateTime);
            service.FilterBy = filterBy;

            var request = new PosReportActivityReqType {
                RptStartUtcDT = utcStartDateTime,
                RptStartUtcDTSpecified = true,
                RptEndUtcDT = utcEndDateTime,
                RptEndUtcDTSpecified = true
            };

            var transaction = new PosRequestVer10Transaction {
                Item = request,
                ItemElementName = ItemChoiceType1.ReportActivity
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsReportTransactionSummary().FromResponse(response, filterBy);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return utcStartDateTime != null; }, "Start DateTime is required.");
            AddValidation(() => { return utcEndDateTime != null; }, "End DateTime is required.");
        }
    }
}
