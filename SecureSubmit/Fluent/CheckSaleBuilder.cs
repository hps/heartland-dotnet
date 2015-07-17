using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CheckSaleBuilder : HpsBuilderAbstract<HpsFluentCheckService, HpsCheckResponse> {
    bool achVerify;
    decimal? amount;
    HpsCheck check;
    bool checkVerify;
    long? clientTransactionId;

    public CheckSaleBuilder WithAchVerify(bool value) {
        this.achVerify = value;
        return this;
    }
    public CheckSaleBuilder WithAmount(decimal? amount) {
        this.amount = amount;
        return this;
    }
    public CheckSaleBuilder WithCheck(HpsCheck check) {
        this.check = check;
        return this;
    }
    public CheckSaleBuilder WithCheckVerify(bool value) {
        this.checkVerify = value;
        return this;
    }
    public CheckSaleBuilder WithClientTransactionId(long? clientTransactionId) {
        this.clientTransactionId = clientTransactionId;
        return this;
    }

    public CheckSaleBuilder(HpsFluentCheckService service) : base(service) {
    }

    public override HpsCheckResponse Execute() {
        base.Execute();

        return service.BuildTransaction(checkActionType.SALE, this.check, this.amount, this.clientTransactionId, this.checkVerify, this.achVerify);
    }

    protected override void SetupValidations() {
        AddValidation(() => { return amount.HasValue; }, "Amount cannot be null.");
        AddValidation(() => { return check != null; }, "Check cannot be null.");
    }
}
}
