using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardActivateBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        decimal? amount;
        HpsGiftCard card;
        string currency;

        public GiftCardActivateBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public GiftCardActivateBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }
        public GiftCardActivateBuilder WithCurrency(string value) {
            this.currency = value;
            return this;
        }

        public GiftCardActivateBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            HpsInputValidation.CheckAmount(amount.Value);
            HpsInputValidation.CheckCurrency(currency);

            var transaction = new PosRequestVer10Transaction{
                Item = new PosGiftCardActivateReqType{
                    Block1 = new GiftCardActivateReqBlock1Type {
                        Amt = amount.Value,
                        CardData = service.HydrateGiftCardData(card)
                    }
                },
                ItemElementName = ItemChoiceType1.GiftCardActivate
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardResponse().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return amount.HasValue; }, "Amount is required.");
            AddValidation(() => { return card != null; }, "Card is required.");
            AddValidation(() => { return currency != null; }, "Currency is required.");
        }
    }
}
