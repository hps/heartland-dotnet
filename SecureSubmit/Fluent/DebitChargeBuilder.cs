using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class DebitChargeBuilder : HpsBuilderAbstract<HpsFluentDebitService, HpsDebitAuthorization> {
    bool allowDuplicates = false;
    bool allowPartialAuth = false;
    decimal? amount;
    HpsCardHolder cardHolder;
    decimal? cashBack;
    string pinBlock;
    string token;
    HpsTrackData trackData;
    HpsTransactionDetails details;

    public DebitChargeBuilder WithAllowDuplicates(bool value) {
        this.allowDuplicates = value;
        return this;
    }
    public DebitChargeBuilder WithAllowPartialAuth(bool value) {
        this.allowPartialAuth = value;
        return this;
    }
    public DebitChargeBuilder WithAmount(decimal? value) {
        this.amount = value;
        return this;
    }
    public DebitChargeBuilder WithCardHolder(HpsCardHolder value) {
        this.cardHolder = value;
        return this;
    }
    public DebitChargeBuilder WithCashBack(decimal? value) {
        this.cashBack = value;
        return this;
    }
    public DebitChargeBuilder WithPinBlock(string value) {
        this.pinBlock = value;
        return this;
    }
    public DebitChargeBuilder WithToken(string value) {
        this.token = value;
        return this;
    }
    public DebitChargeBuilder WithTrackData(HpsTrackData value) {
        this.trackData = value;
        return this;
    }
    public DebitChargeBuilder WithDetails(HpsTransactionDetails value) {
        this.details = value;
        return this;
    }

    public DebitChargeBuilder(HpsFluentDebitService service) : base(service) {
    }

    public override HpsDebitAuthorization Execute() {
        base.Execute();

        HpsInputValidation.CheckAmount(amount.Value);

        var block1 = new DebitSaleReqBlock1Type {
            Amt = amount.Value,
            AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
            AllowDupSpecified = true,
            AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
            AllowPartialAuthSpecified = true
        };

        if(trackData != null) {
            block1.TrackData = trackData.Value;
            if(trackData.EncryptionData != null)
                block1.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
        }
        if (token != null)
            block1.TokenValue = token;

        if (pinBlock != null)
            block1.PinBlock = pinBlock;

        if(cardHolder != null)
            block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

        block1.CashbackAmtInfoSpecified = cashBack.HasValue;
        if (block1.CashbackAmtInfoSpecified)
            block1.CashbackAmtInfo = cashBack.Value;

        if(details != null)
            block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);

        var transaction = new PosRequestVer10Transaction {
            Item = new PosDebitSaleReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.DebitSale
        };

        var clientTxnId = service.GetClientTransactionId(details);
        return service.SubmitTransaction(transaction, clientTxnId);
    }

    protected override void SetupValidations() {
        AddValidation(() => { return amount.HasValue; }, "Amount is required.");
        AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
    }

    private bool OnlyOnePaymentMethod(){
        int count = 0;
        if(trackData != null) count++;
        if(token != null) count++;

        return count == 1;
    }
}

}
