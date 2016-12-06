using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class DebitReturnBuilder : HpsBuilderAbstract<HpsFluentDebitService, HpsDebitAuthorization> {
        bool allowDuplicates = false;
        decimal? amount;
        HpsCardHolder cardHolder;
        HpsTransactionDetails details;
        string pinBlock;
        string token;
        HpsTrackData trackData;
        long? transactionId;

        public DebitReturnBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public DebitReturnBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public DebitReturnBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public DebitReturnBuilder WithDetails(HpsTransactionDetails value) {
            this.details = value;
            return this;
        }
        public DebitReturnBuilder WithPinBlock(string value) {
            this.pinBlock = value;
            return this;
        }
        public DebitReturnBuilder WithToken(string value) {
            this.token = value;
            return this;
        }
        public DebitReturnBuilder WithTrackData(HpsTrackData value) {
            this.trackData = value;
            return this;
        }
        public DebitReturnBuilder WithTransactionId(long? value) {
            this.transactionId = value;
            return this;
        }

        public DebitReturnBuilder(HpsFluentDebitService service)
            : base(service) {
        }

        public override HpsDebitAuthorization Execute() {
            base.Execute();

            HpsInputValidation.CheckAmount(amount.Value);

            var block1 = new DebitReturnReqBlock1Type {
                Amt = amount.Value,
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true,
            };

            if (trackData != null) {
                block1.TrackData = trackData.Value;
                if (trackData.EncryptionData != null)
                    block1.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
            }
            if (transactionId != null) {
                block1.GatewayTxnId = transactionId.Value;
                block1.GatewayTxnIdSpecified = true;
            }
            if (token != null)
                block1.TokenValue = token;

            if (pinBlock != null)
                block1.PinBlock = pinBlock;

            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

            if (details != null)
                block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosDebitReturnReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.DebitReturn
            };

            var clientTxnId = service.GetClientTransactionId(details);
            return service.SubmitTransaction(transaction, clientTxnId);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (trackData != null) count++;
            if (token != null) count++;
            if (transactionId != null) count++;

            return count == 1;
        }
    }

}
