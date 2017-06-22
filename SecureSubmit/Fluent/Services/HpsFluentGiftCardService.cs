using System;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services;

namespace SecureSubmit.Fluent.Services {
    public class HpsFluentGiftCardService : HpsSoapGatewayService {
        public HpsFluentGiftCardService(IHpsServicesConfig config = null, bool enableLogging = false)
            : base(config, enableLogging) {
        }

        public HpsFluentGiftCardService withConfig(IHpsServicesConfig config) {
            this.ServicesConfig = config;
            return this;
        }

        public GiftCardActivateBuilder Activate(decimal? amount = null) {
            return new GiftCardActivateBuilder(this).WithAmount(amount).WithCurrency("USD");
        }

        public GiftCardAddValueBuilder AddValue(decimal? amount) {
            return new GiftCardAddValueBuilder(this).WithAmount(amount).WithCurrency("USD");
        }

        public GiftCardAliasBuilder Alias() {
            return new GiftCardAliasBuilder(this);
        }

        public GiftCardBalanceBuilder Balance() {
            return new GiftCardBalanceBuilder(this);
        }

        public GiftCardDeactivateBuilder Deactivate() {
            return new GiftCardDeactivateBuilder(this);
        }

        public GiftCardReplaceBuilder Replace() {
            return new GiftCardReplaceBuilder(this);
        }

        public GiftCardReverseBuilder Reverse(decimal? amount = null) {
            return new GiftCardReverseBuilder(this).WithAmount(amount);
        }

        public GiftCardRewardBuilder Reward(decimal? amount = null) {
            return new GiftCardRewardBuilder(this).WithAmount(amount).WithCurrency(currencyType.USD);
        }

        public GiftCardSaleBuilder Sale(decimal? amount = null) {
            return new GiftCardSaleBuilder(this).WithAmount(amount).WithCurrency(currencyType.USD);
        }

        public GiftCardVoidBuilder VoidSale(long? transactionId = null) {
            return new GiftCardVoidBuilder(this).WithTransactionId(transactionId);
        }

        public PosResponseVer10 SubmitTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null) {
            var rsp = DoTransaction(transaction, clientTransactionId).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, (ItemChoiceType2)transaction.ItemElementName);

            string rspCode = string.Empty, rspText = string.Empty;
            if (rsp.Transaction != null) {
                var trans = rsp.Transaction.Item;

                var rspCodeField = trans.GetType().GetProperty("RspCode");
                if (rspCodeField != null) {
                    rspCode = rspCodeField.GetValue(trans).ToString();
                }

                var rspTextField = transaction.GetType().GetProperty("RspText");
                if (rspTextField != null) {
                    rspText = rspTextField.GetValue(trans).ToString();
                }
            }
            HpsIssuerResponseValidation.CheckResponse(rsp.Header.GatewayTxnId, rspCode, rspText, HpsCardType.Gift);

            return rsp;
        }
    }
}
