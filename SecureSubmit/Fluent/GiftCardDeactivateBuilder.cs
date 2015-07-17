using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class GiftCardDeactivateBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        HpsGiftCard card;

        public GiftCardDeactivateBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }

        public GiftCardDeactivateBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardDeactivateReqType {
                    Block1 = new GiftCardDeactivateReqBlock1Type {
                        CardData = service.HydrateGiftCardData(card)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardDeactivate
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardResponse().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return card != null; }, "Card is required.");
        }
    }
}
