using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class DebitReverseBuilder : HpsBuilderAbstract<HpsFluentDebitService, HpsDebitAuthorization> {
    decimal? amount;
    decimal? authorizedAmount;
    HpsTransactionDetails details;
    HpsTrackData trackData;
    long? transactionId;

    public DebitReverseBuilder WithAmount(decimal? value) {
        this.amount = value;
        return this;
    }
    public DebitReverseBuilder WithAuthorizedAmount(decimal? value) {
        this.authorizedAmount = value;
        return this;
    }
    public DebitReverseBuilder WithDetails(HpsTransactionDetails value) {
        this.details = value;
        return this;
    }
    public DebitReverseBuilder WithTrackData(HpsTrackData value) {
        this.trackData = value;
        return this;
    }
    public DebitReverseBuilder WithTransactionId(long? value) {
        this.transactionId = value;
        return this;
    }

    public DebitReverseBuilder(HpsFluentDebitService service) : base(service) {
    }

    public override HpsDebitAuthorization Execute() {
        base.Execute();

        HpsInputValidation.CheckAmount(amount.Value);

        var block1 = new DebitReversalReqBlock1Type {
            Amt = amount.Value
        };

        block1.AuthAmtSpecified = authorizedAmount.HasValue;
        if (block1.AuthAmtSpecified)
            block1.AuthAmt = authorizedAmount.Value;

        if (trackData != null) {
            block1.TrackData = trackData.Value;
            if (trackData.EncryptionData != null)
                block1.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
        }
        if (transactionId != null) {
            block1.GatewayTxnId = transactionId.Value;
            block1.GatewayTxnIdSpecified = true;
        }

        if(details != null)
            block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);

        var transaction = new PosRequestVer10Transaction {
            Item = new PosDebitReversalReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.DebitReversal
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
        if(transactionId != null) count++;

        return count == 1;
    }
}

}
