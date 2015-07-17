using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services;

namespace SecureSubmit.Fluent.Services
{
    public class HpsFluentEbtService : HpsSoapGatewayService {
    public HpsFluentEbtService(IHpsServicesConfig config, bool enableLogging = false) : base(config, enableLogging) {
    }

    public HpsFluentEbtService WithConfig(IHpsServicesConfig config) {
        this.ServicesConfig = config;
        return this;
    }

    public EbtBalanceInquiryBuilder BalanceInquiry() {
        return new EbtBalanceInquiryBuilder(this).WithAmount(0.00m);
    }

    public EbtBenefitWithdrawalBuilder BenefitWithdrawl(decimal? amount = null) {
        return new EbtBenefitWithdrawalBuilder(this).WithAmount(amount);
    }

    public EbtCashBackPurchaseBuilder CashBackPurchase(decimal? amount = null) {
        return new EbtCashBackPurchaseBuilder(this).WithAmount(amount);
    }

    public EbtPurchaseBuilder Purchase(decimal? amount = null) {
        return new EbtPurchaseBuilder(this).WithAmount(amount);
    }

    public EbtRefundBuilder Refund(decimal? amount = null) {
        return new EbtRefundBuilder(this).WithAmount(amount);
    }

    public EbtVoucherPurchaseBuilder VoucherPurchase(decimal? amount = null) {
        return new EbtVoucherPurchaseBuilder(this).WithAmount(amount);
    }

    internal HpsEbtAuthorization SubmitTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null) {
        var rsp = DoTransaction(transaction, clientTransactionId).Ver10;

        HpsGatewayResponseValidation.CheckResponse(rsp, (ItemChoiceType2)transaction.ItemElementName);
        
        var authResponse = (AuthRspStatusType)rsp.Transaction.Item;
        HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, authResponse.RspCode, authResponse.RspText);

        return new HpsEbtAuthorization().FromResponse(rsp);
    }
}

}
