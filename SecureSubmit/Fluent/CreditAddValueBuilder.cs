using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditAddValueBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAuthorization> {
        decimal? amount;
        bool allowDuplicates = false;
        HpsCreditCard card;
        HpsCardHolder cardHolder;
        bool requestMultiUseToken = false;
        HpsTrackData trackData;
        HpsTokenData token;

        public CreditAddValueBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditAddValueBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public CreditAddValueBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public CreditAddValueBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public CreditAddValueBuilder WithRequestMultiUseToken(bool value) {
            this.requestMultiUseToken = value;
            return this;
        }
        public CreditAddValueBuilder WithTrackData(HpsTrackData value) {
            this.trackData = value;
            return this;
        }
        public CreditAddValueBuilder WithToken(string token) {
            this.token = new HpsTokenData { TokenValue = token };
            return this;
        }
        public CreditAddValueBuilder WithToken(HpsTokenData token) {
            this.token = token;
            return this;
        }

        public CreditAddValueBuilder(HpsFluentCreditService service) : base(service) { }

        public override HpsAuthorization Execute() {
            base.Execute();

            var block1 = new PrePaidAddValueReqBlock1Type {
                Amt = amount.Value,
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true
            };

            var cardData = new CardDataType();
            if (card != null) {
                cardData.Item = service.HydrateCardManualEntry(card);
                if (card.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(card.EncryptionData);
            }
            else if (trackData != null) {
                cardData.Item = service.HydrateCardTrackData(trackData);
                if (trackData.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
            }
            else if (token != null)
                cardData.Item = service.HydrateTokenData(token);
            cardData.TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N;
            block1.CardData = cardData;

            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosPrePaidAddValueReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.PrePaidAddValue
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsAuthorization().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount != null; }, "Amount is required.");
            AddValidation(onlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool onlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (trackData != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }
}
