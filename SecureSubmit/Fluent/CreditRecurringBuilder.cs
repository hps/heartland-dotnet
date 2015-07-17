using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditRecurringBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAuthorization> {
        HpsPayPlanSchedule schedule;
        string scheduleId;
        decimal? amount;
        HpsCreditCard card;
        string token;
        string paymentMethodKey;
        bool oneTime = false;
        HpsCardHolder cardHolder;
        HpsTransactionDetails details;
        bool allowDuplicates = false;

        public CreditRecurringBuilder WithSchedule(HpsPayPlanSchedule value) {
            this.schedule = value;
            return this;
        }
        public CreditRecurringBuilder WithScheduleId(string value) {
            this.scheduleId = value;
            return this;
        }
        public CreditRecurringBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public CreditRecurringBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public CreditRecurringBuilder WithToken(string value) {
            this.token = value;
            return this;
        }
        public CreditRecurringBuilder WithPaymentMethodKey(string value) {
            this.paymentMethodKey = value;
            return this;
        }
        public CreditRecurringBuilder WithOneTime(bool value) {
            this.oneTime = value;
            return this;
        }
        public CreditRecurringBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public CreditRecurringBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public CreditRecurringBuilder WithDetails(HpsTransactionDetails value) {
            this.details = value;
            return this;
        }

        public CreditRecurringBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsAuthorization Execute() {
            base.Execute();

            var block1 = new RecurringBillReqBlock1Type {
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true,
                Amt = amount.Value
            };
            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);
            if (details != null)
                block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);

            if (card != null) {
                block1.CardData = new CardDataType {
                    Item = service.HydrateCardManualEntry(card)
                };
            }
            if (token != null) {
                block1.CardData = new CardDataType {
                    Item = service.HydrateTokenData(token)
                };
            }
            if (paymentMethodKey != null)
                block1.PaymentMethodKey = paymentMethodKey;

            if (scheduleId == null && schedule != null)
                scheduleId = schedule.ScheduleIdentifier;

            var recurringData = new RecurringDataType {
                OneTime = oneTime ? booleanType.Y : booleanType.N,
                OneTimeSpecified = true
            };
            if (scheduleId != null)
                recurringData.ScheduleID = scheduleId;
            block1.RecurringData = recurringData;

            var transaction = new PosRequestVer10Transaction {
                Item = new PosRecurringBillReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.RecurringBilling
            };

            long? clientTransactionId = service.GetClientTransactionId(details);
            var response = service.SubmitTransaction(transaction, clientTransactionId);
            return new HpsAuthorization().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (paymentMethodKey != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }
}
