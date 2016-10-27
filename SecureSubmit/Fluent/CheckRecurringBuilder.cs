using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CheckRecurringBuilder : HpsBuilderAbstract<HpsFluentCheckService, HpsCheckResponse> {
        decimal? amount;
        string paymentMethodKey;
        string scheduleKey;
        bool oneTime = false;
        long? clientTransactionId;

        public CheckRecurringBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CheckRecurringBuilder WithSchedule(HpsPayPlanSchedule schedule) {
            return WithScheduleId(schedule.ScheduleKey);
        }
        public CheckRecurringBuilder WithScheduleId(string scheduleKey) {
            this.scheduleKey = scheduleKey;
            return this;
        }
        public CheckRecurringBuilder WithPaymentMethodKey(string paymentMethodKey) {
            this.paymentMethodKey = paymentMethodKey;
            return this;
        }
        public CheckRecurringBuilder WithOneTime(bool oneTime) {
            this.oneTime = oneTime;
            return this;
        }
        public CheckRecurringBuilder WithClientTransactionId(long? clientTransactionId)
        {
            this.clientTransactionId = clientTransactionId;
            return this;
        }

        public CheckRecurringBuilder(HpsFluentCheckService service)
            : base(service) {
        }

        public override HpsCheckResponse Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCheckSaleReqType {
                    Block1 = new CheckSaleReqBlock1Type {
                        Amt = amount.Value,
                        AmtSpecified = amount.HasValue,
                        CheckAction = checkActionType.SALE,
                        PaymentMethodKey = paymentMethodKey,
                        RecurringData = new RecurringDataType {
                            OneTime = oneTime ? booleanType.Y : booleanType.N,
                            OneTimeSpecified = true,
                            ScheduleID = scheduleKey ?? null
                        }
                    }
                },
                ItemElementName = ItemChoiceType1.CheckSale
            };

            return service.SubmitTransaction(transaction, clientTransactionId);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return paymentMethodKey != null; }, "Payment method key is required for sale.");
        }
    }
}
