using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class GiftCardAliasBuilder : HpsBuilderAbstract<HpsFluentGiftCardService, HpsGiftCardAlias> {
        HpsGiftCard card;
        string alias;
        GiftCardAliasReqBlock1TypeAction action;

        public GiftCardAliasBuilder WithCard(HpsGiftCard value) {
            this.card = value;
            return this;
        }
        public GiftCardAliasBuilder WithAlias(string value) {
            this.alias = value;
            return this;
        }
        public GiftCardAliasBuilder WithAction(GiftCardAliasReqBlock1TypeAction value) {
            this.action = value;
            return this;
        }

        public GiftCardAliasBuilder(HpsFluentGiftCardService service)
            : base(service) {
        }

        public override HpsGiftCardAlias Execute() {
            base.Execute();

            var block1 = new GiftCardAliasReqBlock1Type {
                Action = action,
                Alias = alias
            };
            if (card != null)
                block1.CardData = service.HydrateGiftCardData(card);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosGiftCardAliasReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.GiftCardAlias
            };

            var response = service.SubmitTransaction(transaction);
            return new HpsGiftCardAlias().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(() => { return alias != null; }, "Alias is required.");
            AddValidation(CardIsNotNull, "Card is required.");
            AddValidation(() => { return action != null; }, "Action is required.");
        }

        private bool CardIsNotNull() {
            if (action != GiftCardAliasReqBlock1TypeAction.CREATE)
                return card != null;
            else return this.card == null;
        }
    }
}
