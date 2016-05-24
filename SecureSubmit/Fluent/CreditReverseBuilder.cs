using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CreditReverseBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsReversal> {
    decimal? amount;
    decimal? authAmount;
    string currency;
    HpsCreditCard card;
    HpsTokenData token;
    int? transactionId;
    HpsTransactionDetails details;

    public CreditReverseBuilder WithAmount(decimal? value) {
        this.amount = value;
        return this;
    }
    public CreditReverseBuilder WithCurrency(string value) {
        this.currency = value;
        return this;
    }
    public CreditReverseBuilder WithCard(HpsCreditCard value) {
        this.card = value;
        return this;
    }
    public CreditReverseBuilder WithToken(string token) {
        this.token = new HpsTokenData { TokenValue = token };
        return this;
    }
    public CreditReverseBuilder WithToken(HpsTokenData token) {
        this.token = token;
        return this;
    }
    public CreditReverseBuilder WithTransactionId(int? value) {
        this.transactionId = value;
        return this;
    }
    public CreditReverseBuilder WithAuthAmount(decimal? value) {
        this.authAmount = value;
        return this;
    }
    public CreditReverseBuilder WithDetails(HpsTransactionDetails value) {
        this.details = value;
        return this;
    }

    public CreditReverseBuilder(HpsFluentCreditService service) : base(service) {
    }

    public override HpsReversal Execute() {
        base.Execute();

        var block1 = new CreditReversalReqBlock1Type {
            Amt = amount.Value,
        };

        block1.AuthAmtSpecified = authAmount.HasValue;
        if (block1.AuthAmtSpecified)
            block1.AuthAmt = authAmount.Value;

        if (card != null) {
            block1.CardData = new CardDataType {
                Item = service.HydrateCardManualEntry(card)
            };
        }
        else if (token != null) {
            block1.CardData = new CardDataType {
                Item = service.HydrateTokenData(token)
            };
        }
        else if (transactionId != null) {
            block1.GatewayTxnId = transactionId.Value;
            block1.GatewayTxnIdSpecified = true;
        }

        if (details != null)
            block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);

        var transaction = new PosRequestVer10Transaction {
            Item = new PosCreditReversalReqType {
                Block1 = block1
            },
            ItemElementName = ItemChoiceType1.CreditReversal
        };

        var clientTransactionId = service.GetClientTransactionId(details);
        var response = service.SubmitTransaction(transaction, clientTransactionId);
        return new HpsReversal().FromResponse(response);
    }

    protected override void SetupValidations() {
        AddValidation(() => { return amount.HasValue; }, "Amount is required.");
        AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
    }

    private bool OnlyOnePaymentMethod(){
        int count = 0;
        if(card != null) count++;
        if(transactionId != null) count++;
        if(token != null) count++;

        return count == 1;
    }
}

}
