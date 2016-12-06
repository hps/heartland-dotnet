using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class GiftCardReverseBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
    HpsGiftCard card;
    decimal? amount;
    string currency;
    long? transactionId;
    long? clientTransactionId;

    public GiftCardReverseBuilder WithAmount(decimal? value) {
        this.amount = value;
        return this;
    }
    public GiftCardReverseBuilder WithCard(HpsGiftCard value) {
        this.card = value;
        return this;
    }
    public GiftCardReverseBuilder WithCurrncy(string value) {
        this.currency = value;
        return this;
    }
    public GiftCardReverseBuilder WithTransactionId(long? value) {
        this.transactionId = value;
        return this;
    }
    public GiftCardReverseBuilder WithClientTransactionId(long? value) {
        this.clientTransactionId = value;
        return this;
    }

    public GiftCardReverseBuilder(HpsFluentGiftCardService service) : base(service) {
    }

    public override HpsGiftCardResponse Execute() {
        base.Execute();

        HpsInputValidation.CheckAmount(amount.Value);

        var block1 = new GiftCardReversalReqBlock1Type {
            Amt = amount.Value
        };

        if (card != null)
            block1.CardData = service.HydrateGiftCardData(card);
        else if (transactionId != null) {
            block1.GatewayTxnId = transactionId.Value;
            block1.GatewayTxnIdSpecified = true;
        }
        else if (clientTransactionId != null) {
            block1.ClientTxnId = clientTransactionId.Value;
            block1.ClientTxnIdSpecified = true;
        }

        var transaction = new PosRequestVer10Transaction {
            Item = new PosGiftCardReversalReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.GiftCardReversal
        };

        var response = service.SubmitTransaction(transaction);
        return new HpsGiftCardResponse().FromResponse(response);
    }

    protected override void SetupValidations() {
        AddValidation(() => { return amount.HasValue; }, "Amount is required.");
        AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
    }

    private bool OnlyOnePaymentMethod(){
        int count = 0;
        if(card != null) count++;
        if(transactionId != null) count++;

        return count == 1;
    }
}

}
