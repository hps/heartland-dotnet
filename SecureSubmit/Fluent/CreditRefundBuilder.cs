using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditRefundBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsRefund> {
        decimal? amount;
        string currency;
        HpsCreditCard card;
        HpsTokenData token;
        int? transactionId;
        HpsCardHolder cardHolder;
        HpsTransactionDetails details;
        bool allowDuplicates = false;
        HpsDirectMarketData directMarketData;

        public CreditRefundBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public CreditRefundBuilder WithCurrency(string value) {
            this.currency = value;
            return this;
        }
        public CreditRefundBuilder WithCard(HpsCreditCard value) {
            this.card = value;
            return this;
        }
        public CreditRefundBuilder WithToken(string token) {
            this.token = new HpsTokenData { TokenValue = token };
            return this;
        }
        public CreditRefundBuilder WithToken(HpsTokenData token) {
            this.token = token;
            return this;
        }
        public CreditRefundBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditRefundBuilder WithCardHolder(HpsCardHolder value) {
            this.cardHolder = value;
            return this;
        }
        public CreditRefundBuilder WithDetails(HpsTransactionDetails value) {
            this.details = value;
            return this;
        }
        public CreditRefundBuilder WithAllowDuplicates(bool value) {
            this.allowDuplicates = value;
            return this;
        }
        public CreditRefundBuilder WithDirectMarketData(HpsDirectMarketData value) {
            this.directMarketData = value;
            return this;
        }

        public CreditRefundBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsRefund Execute() {
            base.Execute();

            var block1 = new CreditReturnReqBlock1Type {
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true,
                Amt = amount.Value
            };

            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

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
            if (directMarketData != null)
                block1.DirectMktData = service.HydrateDirectMktData(directMarketData);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCreditReturnReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CreditReturn
            };

            var clientTransactionId = service.GetClientTransactionId(details);
            var response = service.SubmitTransaction(transaction, clientTransactionId);
            HpsRefund trans = new HpsRefund().FromResponse(response);
            trans.ResponseCode = "00";
            trans.ResponseText = string.Empty;
            return trans;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null) count++;
            if (transactionId != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }
}
