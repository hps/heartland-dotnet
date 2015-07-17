using System;
using SecureSubmit.Entities;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Fluent.Services;

namespace SecureSubmit.Fluent {
    public class EbtRefundBuilder : HpsBuilderAbstract<HpsFluentEbtService, HpsEbtAuthorization> {
        bool allowDuplicates = false;
        decimal? amount;
        HpsCreditCard card;
        HpsCardHolder cardHolder;
        string pinBlock;
        bool requestMultiUseToken = false;
        HpsTrackData trackData;
        string token;
        string tokenParameters;

        public EbtRefundBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public EbtRefundBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public EbtRefundBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public EbtRefundBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public EbtRefundBuilder WithPinBlock(string value) {
            this.pinBlock = value;
            return this;
        }
        public EbtRefundBuilder WithRequestMultiUseToken(bool value) {
            this.requestMultiUseToken = value;
            return this;
        }
        public EbtRefundBuilder WithTrackData(HpsTrackData value) {
            this.trackData = value;
            return this;
        }
        public EbtRefundBuilder WithToken(string value) {
            this.token = value;
            return this;
        }
        public EbtRefundBuilder WithTokenParameters(string value) {
            this.tokenParameters = value;
            return this;
        }

        public EbtRefundBuilder(HpsFluentEbtService service) : base(service) { }

        public override HpsEbtAuthorization Execute() {
            base.Execute();

            var block1 = new EBTFSReturnReqBlock1Type {
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
            if (token != null)
                cardData.Item = service.HydrateTokenData(token);
            if (trackData != null) {
                cardData.Item = service.HydrateCardTrackData(trackData);
                if (trackData.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
            }
            cardData.TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N;
            block1.CardData = cardData;
            block1.PinBlock = pinBlock;

            var transaction = new PosRequestVer10Transaction {
                Item = new PosEBTFSReturnReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.EBTFSReturn
            };

            return service.SubmitTransaction(transaction);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return pinBlock != null; }, "Pin block is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (trackData != null) count++;
            if (card != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }
}
