using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services;

namespace SecureSubmit.Fluent.Services {
    public class HpsFluentDebitService : HpsSoapGatewayService {
        public HpsFluentDebitService() { }
        public HpsFluentDebitService(IHpsServicesConfig config, bool enableLogging = false)
            : base(config, enableLogging) {
        }

        public HpsFluentDebitService withConfig(IHpsServicesConfig config) {
            this.ServicesConfig = config;
            return this;
        }

        public DebitChargeBuilder Charge(decimal? amount = null) {
            return new DebitChargeBuilder(this).WithAmount(amount);
        }

        public DebitReturnBuilder Refund(decimal? amount) {
            return new DebitReturnBuilder(this).WithAmount(amount);
        }

        public DebitReverseBuilder Reverse(decimal? amount) {
            return new DebitReverseBuilder(this).WithAmount(amount);
        }

        internal HpsDebitAuthorization SubmitTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null) {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;

            decimal? amount = null;
            if (transaction.ItemElementName == ItemChoiceType1.DebitSale)
                amount = ((PosDebitSaleReqType)transaction.Item).Block1.Amt;

            ProcessGatewayResponse(rsp, transaction.ItemElementName, amount);
            ProcessIssuerResponse(rsp, transaction.ItemElementName, amount);

            return new HpsDebitAuthorization().FromResponse(rsp);
        }

        private void ProcessIssuerResponse(PosResponseVer10 response, ItemChoiceType1 expectedType, decimal? amount) {
            long transactionId = response.Header.GatewayTxnId;

            if (!(response.Transaction.Item is AuthRspStatusType))
                return;

            var transaction = (AuthRspStatusType)response.Transaction.Item;
            if (transaction.RspCode == "91") {
                try {
                    //this.Reverse(amount).Execute();
                }
                catch (HpsGatewayException e) {
                    if (e.Details.GatewayResponseCode == 3)
                        HpsIssuerResponseValidation.CheckResponse(transactionId, transaction.RspCode, transaction.RspText);
                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError, "Error occurred while reversing a charge due to an issuer timeout.", e);
                }
                catch (HpsException e) {
                    throw new HpsCreditException(transactionId, HpsExceptionCodes.IssuerTimeoutReversalError, "Error occurred while reversing a charge due to an issuer timeout.", e);
                }
            }
            HpsIssuerResponseValidation.CheckResponse(transactionId, transaction.RspCode, transaction.RspText);
        }

        private void ProcessGatewayResponse(PosResponseVer10 response, ItemChoiceType1 expectedResponseType, decimal? amount) {
            var responseCode = response.Header.GatewayRspCode;
            var transactionId = response.Header.GatewayTxnId;
            if (responseCode == 0)
                return;

            if (responseCode == 30) {
                try {
                    //Reverse(amount).WithTransactionId(transactionId).Execute();
                }
                catch (HpsException e) {
                    throw new HpsGatewayException(HpsExceptionCodes.GatewayTimeoutReversalError, "Error occurred while reversing a charge due to a gateway timeout.", e);
                }
            }
            HpsGatewayResponseValidation.CheckResponse(response, (ItemChoiceType2)expectedResponseType);
        }
    }
}
