using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardVoidBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        int? transactionId;

        public GiftCardVoidBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }

        public GiftCardVoidBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardVoidReqType {
                    Block1 = new GiftCardVoidReqBlock1Type {
                        GatewayTxnId = transactionId.Value
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardVoid
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardResponse().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "Transaction ID is required.");
        }
    }
}
