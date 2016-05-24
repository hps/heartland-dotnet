using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CreditBalanceInquiryBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAuthorization> {
    HpsCreditCard card;
    HpsCardHolder cardHolder;
    HpsTrackData trackData;
    HpsTokenData token;

    public CreditBalanceInquiryBuilder WithCard(HpsCreditCard card) {
        this.card = card;
        return this;
    }
    public CreditBalanceInquiryBuilder WithCardHolder(HpsCardHolder cardHolder) {
        this.cardHolder = cardHolder;
        return this;
    }
    public CreditBalanceInquiryBuilder WithTrackData(HpsTrackData trackData) {
        this.trackData = trackData;
        return this;
    }
    public CreditBalanceInquiryBuilder WithToken(string token) {
        this.token = new HpsTokenData { TokenValue = token };
        return this;
    }
    public CreditBalanceInquiryBuilder WithToken(HpsTokenData token) {
        this.token = token;
        return this;
    }

    public CreditBalanceInquiryBuilder(HpsFluentCreditService service) : base(service) {
    }

    public override HpsAuthorization Execute() {
        base.Execute();

        var block1 = new PrePaidBalanceInquiryReqBlock1Type();

        if (cardHolder != null)
            block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

        var cardData = new CardDataType();
        if (card != null)
            cardData.Item = service.HydrateCardManualEntry(card);
        if(trackData != null)
            cardData.Item = service.HydrateCardTrackData(trackData);
        if (token != null)
            cardData.Item = service.HydrateTokenData(token);
        block1.CardData = cardData;

        var transaction = new PosRequestVer10Transaction {
            Item = new PosPrePaidBalanceInquiryReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.PrePaidBalanceInquiry
        };

        var response = service.SubmitTransaction(transaction);
        return new HpsAuthorization().FromResponse(response);
    }

    protected override void SetupValidations() {
        AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
    }

    private bool OnlyOnePaymentMethod(){
        int count = 0;
        if(card != null) count++;
        if(trackData != null) count++;
        if(token != null) count++;

        return count == 1;
    }
}

}
