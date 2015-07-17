using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;

namespace SecureSubmit.Fluent {
    public class EbtVoucherPurchaseBuilder : HpsBuilderAbstract<HpsFluentEbtService, HpsEbtAuthorization> {
        bool allowDuplicates = false;
        decimal? amount;
        HpsCreditCard card;
        HpsCardHolder cardHolder;
        string pinBlock;
        bool requestMultiUseToken = false;
        HpsTrackData trackData;
        string token;
        string tokenParameters;

        string approvalCode;
        string expirationDate = "";
        string primaryAccountNumber = "";
        string serialNumber;

        public EbtVoucherPurchaseBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithPinBlock(string value) {
            this.pinBlock = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithRequestMultiUseToken(bool value) {
            this.requestMultiUseToken = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithTrackData(HpsTrackData value) {
            this.trackData = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithToken(string value) {
            this.token = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithTokenParameters(string value) {
            this.tokenParameters = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithApprovalCode(string value) {
            this.approvalCode = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithExpirationDate(string value) {
            this.expirationDate = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithPrimaryAccountNumber(string value) {
            this.primaryAccountNumber = value;
            return this;
        }
        public EbtVoucherPurchaseBuilder WithSerialNumber(string value) {
            this.serialNumber = value;
            return this;
        }

        public EbtVoucherPurchaseBuilder(HpsFluentEbtService service) : base(service) { }

        public override HpsEbtAuthorization Execute() {
            base.Execute();

            var block1 = new EBTFSVoucherReqBlock1Type {
                Amt = amount.Value,
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true
            };

            block1.ElectronicVoucherSerialNbr = serialNumber;
            block1.VoucherApprovalCd = approvalCode;
            block1.ExprDate = expirationDate;
            block1.PrimaryAcctNbr = primaryAccountNumber;

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
                Item = new PosEBTFSVoucherReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.EBTVoucherPurchase
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
