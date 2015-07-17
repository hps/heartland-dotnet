using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardReplaceBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        HpsGiftCard oldCard;
        HpsGiftCard newCard;

        public GiftCardReplaceBuilder WithNewCard(HpsGiftCard value) {
            this.newCard = value;
            return this;
        }
        public GiftCardReplaceBuilder WithOldCard(HpsGiftCard value) {
            this.oldCard = value;
            return this;
        }

        public GiftCardReplaceBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardReplaceReqType {
                    Block1 = new GiftCardReplaceReqBlock1Type {
                        NewCardData = service.HydrateGiftCardData(newCard, "NewCardData"),
                        OldCardData = service.HydrateGiftCardData(oldCard, "OldCardData")
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardReplace
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardResponse().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return oldCard != null; }, "OldCard is required.");
            AddValidation(() => { return newCard != null; }, "NewCard is required.");
        }
    }
}