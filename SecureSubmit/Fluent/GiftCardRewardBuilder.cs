using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardRewardBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardResponse> {
        HpsGiftCard card;
        decimal? amount;
        currencyType currency;
        decimal? gratuity;
        decimal? tax;

        public GiftCardRewardBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public GiftCardRewardBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }
        public GiftCardRewardBuilder WithCurrency(currencyType value) {
            this.currency = value;
            return this;
        }
        public GiftCardRewardBuilder WithGratuity(decimal? value) {
            this.gratuity = value;
            return this;
        }
        public GiftCardRewardBuilder WithTax(decimal? value) {
            this.tax = value;
            return this;
        }

        public GiftCardRewardBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardResponse Execute() {
            base.Execute();

            HpsInputValidation.CheckAmount(amount.Value);
            
            var block1 = new GiftCardRewardReqBlock1Type {
                Amt = amount.Value,
                CardData = service.HydrateGiftCardData(card),
                Currency = currency
            };

            block1.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (block1.GratuityAmtInfoSpecified)
                block1.GratuityAmtInfo = gratuity.Value;

            block1.TaxAmtInfoSpecified = tax.HasValue;
            if (tax != null)
                block1.TaxAmtInfo = tax.Value;

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardRewardReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.GiftCardReward
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
