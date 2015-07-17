using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;

namespace SecureSubmit.Fluent {
    public class EbtBalanceInquiryBuilder : HpsBuilderAbstract<HpsFluentEbtService, HpsEbtAuthorization> {
    decimal? amount;
    HpsCreditCard card;
    bool cardPresent = false;
    EBTBalanceInquiryType inquiryType;
    string pinBlock;
    bool readerPresent = false;
    bool requestMultiUseToken = false;
    HpsTrackData trackData;
    string token;
    string tokenParameters;

    public EbtBalanceInquiryBuilder WithAmount(decimal? value) {
        this.amount = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithCard(HpsCreditCard value) {
        this.card = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithCardPresent(bool value) {
        this.cardPresent = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithInquiryType(EBTBalanceInquiryType value) {
        this.inquiryType = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithPinBlock(string value) {
        this.pinBlock = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithReaderPresent(bool value) {
        this.readerPresent = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithRequestMultiUseToken(bool value) {
        this.requestMultiUseToken = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithTrackData(HpsTrackData value) {
        this.trackData = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithToken(string value) {
        this.token = value;
        return this;
    }
    public EbtBalanceInquiryBuilder WithTokenParameters(string value) {
        this.tokenParameters = value;
        return this;
    }

    public EbtBalanceInquiryBuilder(HpsFluentEbtService service) : base(service) { }

    public override HpsEbtAuthorization Execute() {
        base.Execute();

        var block1 = new EBTBalanceInquiryReqBlock1Type {
            Amt = amount.Value
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
        block1.BalanceInquiryType = inquiryType;

        var transaction = new PosRequestVer10Transaction {
            Item = new PosEBTBalanceInquiryReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.EBTBalanceInquiry
        };

        return service.SubmitTransaction(transaction);
    }

    protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return pinBlock != null; }, "Pin block is required.");
            AddValidation(() => { return inquiryType != null; }, "Inquiry Type is required.");
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
