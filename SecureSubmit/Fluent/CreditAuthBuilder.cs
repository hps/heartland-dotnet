using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent
{
    public class CreditAuthBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsAuthorization>
    {
        private decimal? amount;
        private string currency;
        private HpsCreditCard card;
        private HpsTokenData token;
        private HpsTrackData trackData;
        private HpsCardHolder cardHolder;
        private bool requestMultiUseToken = false;
        private HpsTransactionDetails details;
        private string txnDescriptor;
        private bool allowPartialAuth = false;
        private bool cpcReq = false;
        private HpsDirectMarketData directMarketData;
        private bool allowDuplicates = false;
        private bool cardPresent = false;
        private bool readerPresent = false;
        private decimal? gratuity;
        private decimal? convenienceAmt;
        private decimal? shippingAmt;
        private HpsAutoSubstantiation autoSubstantiation;
        private HpsTxnReferenceData originalTxnReferenceData;        
        private HpsTagDataType tagData;
        private HpsSecureEcommerce secureEcommerce;

        public CreditAuthBuilder WithAmount(decimal? amount)
        {
            this.amount = amount;
            return this;
        }
        public CreditAuthBuilder WithCurrency(string currency)
        {
            this.currency = currency;
            return this;
        }
        public CreditAuthBuilder WithCard(HpsCreditCard card)
        {
            this.card = card;
            return this;
        }
        public CreditAuthBuilder WithToken(string token)
        {
            this.token = new HpsTokenData { TokenValue = token };
            return this;
        }
        public CreditAuthBuilder WithToken(HpsTokenData token)
        {
            this.token = token;
            return this;
        }
        public CreditAuthBuilder WithTrackData(HpsTrackData trackData)
        {
            this.trackData = trackData;
            return this;
        }       
        public CreditAuthBuilder WithTagData(HpsTagDataType tagData)
        {
            this.tagData = tagData;
            return this;
        }
        public CreditAuthBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            this.cardHolder = cardHolder;
            return this;
        }
        public CreditAuthBuilder WithRequestMultiUseToken(bool requestMultiUseToken)
        {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditAuthBuilder WithDetails(HpsTransactionDetails details)
        {
            this.details = details;
            return this;
        }
        public CreditAuthBuilder WithTxnDescriptor(string txnDescriptor)
        {
            this.txnDescriptor = txnDescriptor;
            return this;
        }
        public CreditAuthBuilder WithAllowPartialAuth(bool allowPartialAuth)
        {
            this.allowPartialAuth = allowPartialAuth;
            return this;
        }
        public CreditAuthBuilder WithCpcReq(bool cpcReq)
        {
            this.cpcReq = cpcReq;
            return this;
        }
        public CreditAuthBuilder WithAllowDuplicates(bool allowDuplicates)
        {
            this.allowDuplicates = allowDuplicates;
            return this;
        }
        public CreditAuthBuilder WithCardPresent(bool cardPresent)
        {
            this.cardPresent = cardPresent;
            return this;
        }
        public CreditAuthBuilder WithReaderPresent(bool readerPresent)
        {
            this.readerPresent = readerPresent;
            return this;
        }
        public CreditAuthBuilder WithGratuity(decimal? gratuity)
        {
            this.gratuity = gratuity;
            return this;
        }
        public CreditAuthBuilder WithShippingAmt(decimal? shippingAmt)
        {
            this.shippingAmt = shippingAmt;
            return this;
        }
        public CreditAuthBuilder WithConvenienceAmt(decimal? convenienceAmt)
        {
            this.convenienceAmt = convenienceAmt;
            return this;
        }
        public CreditAuthBuilder WithAutoSubstantiation(HpsAutoSubstantiation autoSubstantiation)
        {
            this.autoSubstantiation = autoSubstantiation;
            return this;
        }
        public CreditAuthBuilder WithOriginalTxnReferenceData(HpsTxnReferenceData originalTxnReferenceData)
        {
            this.originalTxnReferenceData = originalTxnReferenceData;
            return this;
        }
        public CreditAuthBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            this.directMarketData = directMarketData;
            return this;
        }
        public CreditAuthBuilder WithSecureEcommerce(HpsSecureEcommerce secureEcommerce)
        {
            this.secureEcommerce = secureEcommerce;
            return this;
        }

        public CreditAuthBuilder(HpsFluentCreditService service) : base(service) {}

        public override HpsAuthorization Execute() {
            base.Execute();

            var block1 = new CreditAuthReqBlock1Type {
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true,
                AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                AllowPartialAuthSpecified = true,
                Amt = amount.Value,
            };

            block1.GratuityAmtInfoSpecified = gratuity.HasValue;
            if (block1.GratuityAmtInfoSpecified)
                block1.GratuityAmtInfo = gratuity.Value;

            block1.ConvenienceAmtInfoSpecified = convenienceAmt.HasValue;
            if (block1.ConvenienceAmtInfoSpecified)
            {
                block1.ConvenienceAmtInfo = convenienceAmt.Value;
                HpsInputValidation.CheckAmount(block1.ConvenienceAmtInfo);
            }
            block1.ShippingAmtInfoSpecified = shippingAmt.HasValue;
            if (block1.ShippingAmtInfoSpecified)
            {
                block1.ShippingAmtInfo = shippingAmt.Value;
                HpsInputValidation.CheckAmount(block1.ShippingAmtInfo);
            }
            if (cardHolder != null)
                block1.CardHolderData = service.HydrateCardHolderData(cardHolder);

            var cardData = new CardDataType();
            if (card != null) {
                cardData.Item = service.HydrateCardManualEntry(card, cardPresent, readerPresent);
                if (card.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(card.EncryptionData);
            }
            if(token != null)
                cardData.Item = service.HydrateTokenData(token, cardPresent, readerPresent);
            if (trackData != null) {
                cardData.Item = service.HydrateCardTrackData(trackData);
                if (trackData.EncryptionData != null)
                    cardData.EncryptionData = service.HydrateEncryptionData(trackData.EncryptionData);
            }
            cardData.TokenRequest = requestMultiUseToken ? booleanType.Y : booleanType.N;

            if (cpcReq) {
                block1.CPCReq = booleanType.Y;
                block1.CPCReqSpecified = true; 
            }

            if (details != null)
                block1.AdditionalTxnFields = service.HydrateAdditionalTxnFields(details);
            if (txnDescriptor != null)
                block1.TxnDescriptor = txnDescriptor;
            if (autoSubstantiation != null)
                block1.AutoSubstantiation = service.HydrateAutoSubstantiation(autoSubstantiation);
            if (originalTxnReferenceData != null)
                block1.OrigTxnRefData = new origTxnRefDataType {
                    AuthCode = originalTxnReferenceData.AuthorizationCode,
                    CardNbrLastFour = originalTxnReferenceData.CardNumberLast4
                };
            if (directMarketData != null)
                block1.DirectMktData = service.HydrateDirectMktData(directMarketData);
            block1.CardData = cardData;

            if (tagData != null)
                block1.TagData = service.HydrateTagData(tagData);

            if (secureEcommerce != null)
                block1.SecureECommerce = service.HydrateSecureEcommerce(secureEcommerce);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCreditAuthReqType {
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CreditAuth
            };

            var clientTransactionId = service.GetClientTransactionId(details);
            var response = service.SubmitTransaction(transaction, clientTransactionId);
            return new HpsAuthorization().FromResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(AmountIsNotNull, "Amount is required.");
            AddValidation(OnlyOnePaymentMethod, "Only one payment method is required.");
        }

        private bool AmountIsNotNull() {
            return this.amount != null;
        }

        private bool OnlyOnePaymentMethod() {
            int count = 0;
            if (card != null)
                count++;
            if (token != null)
                count++;
            if (trackData != null)
                count++;

            return count == 1;
        }
    }
}
