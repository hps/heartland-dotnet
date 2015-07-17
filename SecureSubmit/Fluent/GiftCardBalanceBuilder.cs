using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardBalanceBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        HpsGiftCard card;

        public GiftCardBalanceBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }

        public GiftCardBalanceBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardBalanceReqType {
                    Block1 = new GiftCardBalanceReqBlock1Type {
                        CardData = service.HydrateGiftCardData(card)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardBalance
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardResponse().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return card != null; }, "Card is required.");
        }
    }
}
