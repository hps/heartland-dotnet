using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CreditVerifyBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAccountVerify>
    {
        private HpsCreditCard card;
        private HpsTokenData token;
        private HpsTrackData trackData;
        private HpsCardHolder cardHolder;
        private bool requestMultiUseToken = false;
        private bool cardPresent = false;
        private bool readerPresent = false;
        private long? clientTransactionId;        
        private HpsTagDataType tagData;

        public CreditVerifyBuilder WithCard(HpsCreditCard card)
        {
            this.card = card;
            return this;
        }
        public CreditVerifyBuilder WithToken(string token)
        {
            this.token = new HpsTokenData { TokenValue = token };
            return this;
        }
        public CreditVerifyBuilder WithToken(HpsTokenData token)
        {
            this.token = token;
            return this;
        }
        public CreditVerifyBuilder WithTrackData(HpsTrackData trackData)
        {
            this.trackData = trackData;
            return this;
        }
        public CreditVerifyBuilder WithTagData(HpsTagDataType tagData)
        {
            this.tagData = tagData;
            return this;
        }
        public CreditVerifyBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            this.cardHolder = cardHolder;
            return this;
        }
        public CreditVerifyBuilder WithRequestMultiUseToken(bool requestMultiUseToken)
        {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditVerifyBuilder WithCardPresent(bool cardPresent)
        {
            this.cardPresent = cardPresent;
            return this;
        }
        public CreditVerifyBuilder WithReaderPresent(bool readerPresent)
        {
            this.readerPresent = readerPresent;
            return this;
        }
        public CreditVerifyBuilder WithClientTransactionId(long? clientTransactionId)
        {
            this.clientTransactionId = clientTransactionId;
            return this;
        }

        public CreditVerifyBuilder(HpsFluentCreditService service)
            : base(service)
        {
        }

        public override HpsAccountVerify Execute()
        {
            base.Execute();

            var block1 = new CreditAccountVerifyBlock1Type();

            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

            var cardData = new CardDataType();
            if (card != null)
            {
                cardData.Item = service.HydrateCardManualEntry(card, cardPresent, readerPresent);
                if (card.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(card.EncryptionData);
            }
            if (token != null)
                cardData.Item = service.HydrateTokenData(token, cardPresent, readerPresent);
            if (trackData != null)
            {
                cardData.Item = service.HydrateCardTrackData(trackData);
                if (trackData.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
            }
            cardData.TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N;
            block1.CardData = cardData;

            if (tagData != null)
                block1.TagData = service.HydrateTagData(tagData);

            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCreditAccountVerifyReqType
                {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CreditAccountVerify
            };

            var response = service.SubmitTransaction(transaction, clientTransactionId);
            return new HpsAccountVerify().FromResponse(response);
        }

        protected override void SetupValidations()
        {
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool OnlyOnePaymentMethod()
        {
            int count = 0;
            if (card != null) count++;
            if (trackData != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }

}
