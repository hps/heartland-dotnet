using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditAdditionalAuthBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAuthorization> {
        private bool allowDuplicates = false;
        private decimal? amount;
        private HpsTransactionDetails details;
        private int transactionId;
        private string txnDescriptor;

        public CreditAdditionalAuthBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public CreditAdditionalAuthBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditAdditionalAuthBuilder WithDetails(HpsTransactionDetails details) {
            this.details = details;
            return this;
        }
        public CreditAdditionalAuthBuilder WithTransactionId(int transactionId) {
            this.transactionId = transactionId;
            return this;
        }
        public CreditAdditionalAuthBuilder WithDescriptor(string descriptor) {
            this.txnDescriptor = descriptor;
            return this;
        }

        public CreditAdditionalAuthBuilder(HpsFluentCreditService service) : base(service) { }

        public override HpsAuthorization Execute() {
            base.Execute();

            HpsInputValidation.CheckAmount(amount.Value);
            var block1 = new CreditAdditionalAuthReqBlock1Type{
                Amt = amount.Value,
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                GatewayTxnId = transactionId,
                GatewayTxnIdSpecified = true
            };

            if (details != null)
                block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);
            if (txnDescriptor != null)
                block1.TxnDescriptor = txnDescriptor;

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCreditAdditionalAuthReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CreditAdditionalAuth
            };

            long? clientTransactionId = service.GetClientTransactionId(details);
            var response = service.SubmitTransaction(transaction, clientTransactionId);
            return new HpsAuthorization().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(AmountIsNotNull, "Amount is required.");
        }

        private bool AmountIsNotNull() {
            return amount != null;
        }
    }
}
