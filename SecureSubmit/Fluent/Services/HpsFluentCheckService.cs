using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services;

namespace SecureSubmit.Fluent.Services {
    public class HpsFluentCheckService : HpsSoapGatewayService {
        public HpsFluentCheckService(IHpsServicesConfig config, bool enableLogging = false)
            : base(config, enableLogging) {
        }

        //public CheckOverrideBuilder Override() {
        //    return new CheckOverrideBuilder(this);
        //}

        public CheckRecurringBuilder Recurring(decimal? amount) {
            return new CheckRecurringBuilder(this).WithAmount(amount);
        }

        //public CheckReturnBuilder ReturnCheck() {
        //    return new CheckReturnBuilder(this);
        //}

        public CheckSaleBuilder Sale(decimal? amount) {
            return new CheckSaleBuilder(this).WithAmount(amount);
        }

        public CheckVoidBuilder Void() {
            return new CheckVoidBuilder(this);
        }

        public HpsCheckResponse BuildTransaction(checkActionType action, HpsCheck check, decimal? amount, long? clientTransactionId = null, bool checkVerify = false, bool achVerify = false) {
            if (amount.HasValue)
                HpsInputValidation.CheckAmount(amount.Value);

            if (check.SecCode == "CCD" && (check.CheckHolder == null || check.CheckHolder.CheckName == null))
                throw new HpsInvalidRequestException(HpsExceptionCodes.MissingCheckName, "For SEC Code CCD the check name is required.", "CheckName");

            var block1 = new CheckSaleReqBlock1Type {
                CheckAction = action,
                SECCode = check.SecCode,
                AccountInfo = HydrateCheckData(check),
                AmtSpecified = amount.HasValue,
                CheckTypeSpecified = check.CheckType.HasValue,
                DataEntryModeSpecified = check.DataEntryMode.HasValue,
                VerifyInfo = new VerifyInfoType {
                    ACHVerify = achVerify ? booleanType.Y : booleanType.N,
                    ACHVerifySpecified = true,
                    CheckVerify = checkVerify ? booleanType.Y : booleanType.N,
                    CheckVerifySpecified = true
                }
            };

            if (block1.AmtSpecified)
                block1.Amt = amount.Value;

            if (block1.CheckTypeSpecified)
                block1.CheckType = check.CheckType.Value;
            if (block1.DataEntryModeSpecified)
                block1.DataEntryMode = check.DataEntryMode.Value;
            if (check.CheckHolder != null)
                block1.ConsumerInfo = HydrateConsumerInfo(check.CheckHolder);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCheckSaleReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CheckSale
            };

            return SubmitTransaction(transaction, clientTransactionId);
        }

        public HpsCheckResponse SubmitTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null) {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, (ItemChoiceType2)transaction.ItemElementName);

            HpsCheckResponse response = new HpsCheckResponse().FromResponse(rsp);

            int responseCode;
            string responseMsg = string.Empty;
            if (rsp.Transaction.Item is PosCheckSaleRspType) {
                var item = (PosCheckSaleRspType)rsp.Transaction.Item;
                responseCode = item.RspCode;
                responseMsg = item.RspMessage;
            }
            else if (rsp.Transaction.Item is PosCheckVoidRspType) {
                var item = (PosCheckVoidRspType)rsp.Transaction.Item;
                responseCode = item.RspCode;
                responseMsg = item.RspMessage;
            }
            else {
                var item = (AuthRspStatusType)rsp.Transaction.Item;
                responseCode = int.Parse(item.RspCode);
                responseMsg = item.RspText;
            }

            if (responseCode == null || responseCode != 0)
                throw new HpsCheckException(
                        rsp.Header.GatewayTxnId,
                        response.Details,
                        responseCode,
                        responseMsg
                );

            return response;
        }
    }
}
