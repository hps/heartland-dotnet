using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services;

namespace SecureSubmit.Fluent.Services
{
    public class HpsFluentCreditService : HpsSoapGatewayService
    {
        internal HpsTransactionType FilterBy { get; set; }

        public HpsFluentCreditService(IHpsServicesConfig config = null, bool enableLogging = false) : base(config, enableLogging) { }

        public HpsFluentCreditService WithConfig(IHpsServicesConfig config) { this.ServicesConfig = config; return this; }

        public CreditAuthBuilder Authorize(decimal? amount = null)
        {
            return new CreditAuthBuilder(this).WithAmount(amount).WithCurrency("USD");
        }

        public CreditAdditionalAuthBuilder AdditionalAuth(decimal? amount = null) {
            return new CreditAdditionalAuthBuilder(this).WithAmount(amount);
        }

        public CreditCaptureBuilder Capture(long? transactionId = null) {
            return new CreditCaptureBuilder(this).WithTransactionId(transactionId);
        }

        public CreditChargeBuilder Charge(decimal? amount = null) {
            return new CreditChargeBuilder(this).WithAmount(amount).WithCurrency("USD");
        }

        public CreditCpcEditBuilder CpcEdit(long? transactionId = null) {
            return new CreditCpcEditBuilder(this).WithTransactionId(transactionId);
        }

        public CreditEditBuilder Edit(long? transactionId = null) {
            return new CreditEditBuilder(this).WithTransactionId(transactionId);
        }

        public CreditGetBuilder Get(long? transactionId = null) {
            return new CreditGetBuilder(this).WithTransactionId(transactionId);
        }

        public CreditListBuilder List() {
            return new CreditListBuilder(this);
        }

        public CreditOfflineAuthBuilder OfflineAuth(decimal? amount = null) {
            return new CreditOfflineAuthBuilder(this).WithAmount(amount);
        }

        public CreditOfflineChargeBuilder OfflineCharge(decimal? amount = null) {
            return new CreditOfflineChargeBuilder(this).WithAmount(amount);
        }

        public CreditAddValueBuilder PrePaidAddValue(decimal? amount = null) {
            return new CreditAddValueBuilder(this).WithAmount(amount);
        }

        public CreditBalanceInquiryBuilder PrePaidBalanceInquiry() {
            return new CreditBalanceInquiryBuilder(this);
        }

        public CreditRecurringBuilder Recurring(decimal? amount = null) {
            return new CreditRecurringBuilder(this).WithAmount(amount);
        }

        public CreditRefundBuilder Refund(decimal? amount = null) {
            return new CreditRefundBuilder(this).WithAmount(amount);
        }

        public CreditReverseBuilder Reverse(decimal? amount = null) {
            return new CreditReverseBuilder(this).WithAmount(amount);
        }

        public CreditVerifyBuilder Verify() {
            return new CreditVerifyBuilder(this);
        }

        public CreditVoidBuilder Void(long? transactionId = null) {
            return new CreditVoidBuilder(this).WithTransactionId(transactionId);
        }

        public PosResponseVer10 SubmitTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null)
        {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;

            decimal? amount = null;
            if (transaction.ItemElementName == ItemChoiceType1.CreditAuth || transaction.ItemElementName == ItemChoiceType1.CreditSale)
            {
                if (transaction.ItemElementName == ItemChoiceType1.CreditAuth)
                    amount = ((PosCreditAuthReqType)transaction.Item).Block1.Amt;
                else amount = ((PosCreditSaleReqType)transaction.Item).Block1.Amt;
            }

            ProcessGatewayResponse(rsp, transaction.ItemElementName, amount);
            ProcessIssuerResponse(rsp, transaction.ItemElementName, amount);

            return rsp;
        }

        private void ProcessIssuerResponse(PosResponseVer10 response, ItemChoiceType1 expectedType, decimal? amount)
        {
            long transactionId = response.Header.GatewayTxnId;

            if (!(response.Transaction.Item is AuthRspStatusType))
                return;

            var transaction = (AuthRspStatusType)response.Transaction.Item;
            if (transaction.RspCode == "91")
            {
                try
                {
                    //this.Reverse(amount).Execute();
                }
                catch (HpsGatewayException e)
                {
                    if (e.Details.GatewayResponseCode == 3)
                        HpsIssuerResponseValidation.CheckResponse(transactionId, transaction.RspCode, transaction.RspText);
                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError, "Error occurred while reversing a charge due to an issuer timeout.", e);
                }
                catch (HpsException e)
                {
                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError, "Error occurred while reversing a charge due to an issuer timeout.", e);
                }
            }
            HpsIssuerResponseValidation.CheckResponse(transactionId, transaction.RspCode, transaction.RspText);
        }

        private void ProcessGatewayResponse(PosResponseVer10 response, ItemChoiceType1 expectedResponseType, decimal? amount)
        {
            var responseCode = response.Header.GatewayRspCode;
            var transactionId = response.Header.GatewayTxnId;
            if (responseCode == 0)
                return;

            if (responseCode == 30)
            {
                try
                {
                    //Reverse(amount).WithTransactionId(transactionId).Execute();
                }
                catch (HpsException e)
                {
                    throw new HpsGatewayException(HpsExceptionCodes.GatewayTimeoutReversalError, "Error occurred while reversing a charge due to a gateway timeout.", e);
                }
            }
            HpsGatewayResponseValidation.CheckResponse(response, (ItemChoiceType2)expectedResponseType);
        }
    }
}
