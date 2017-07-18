using System.Collections.Generic;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditChargeBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsCharge> {
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
        private HpsEmvDataType emvData;
        private HpsGiftCard rewards;
        private HpsSecureEcommerce secureEcommerce;

        public CreditChargeBuilder WithAmount(decimal? amount) {
            this.amount = amount;
            return this;
        }
        public CreditChargeBuilder WithCurrency(string currency) {
            this.currency = currency;
            return this;
        }
        public CreditChargeBuilder WithCard(HpsCreditCard card) {
            this.card = card;
            return this;
        }
        public CreditChargeBuilder WithToken(string token) {
            this.token = new HpsTokenData { TokenValue = token };
            return this;
        }
        public CreditChargeBuilder WithToken(HpsTokenData token) {
            this.token = token;
            return this;
        }
        public CreditChargeBuilder WithTrackData(HpsTrackData trackData) {
            this.trackData = trackData;
            return this;
        }
        public CreditChargeBuilder WithEMVData(HpsEmvDataType emvData)
        {
            this.emvData = emvData;
            return this;
        }
        public CreditChargeBuilder WithCardHolder(HpsCardHolder cardHolder) {
            this.cardHolder = cardHolder;
            return this;
        }
        public CreditChargeBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            this.requestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public CreditChargeBuilder WithDetails(HpsTransactionDetails details) {
            this.details = details;
            return this;
        }
        public CreditChargeBuilder WithTxnDescriptor(string txnDescriptor) {
            this.txnDescriptor = txnDescriptor;
            return this;
        }
        public CreditChargeBuilder WithAllowPartialAuth(bool allowPartialAuth) {
            this.allowPartialAuth = allowPartialAuth;
            return this;
        }
        public CreditChargeBuilder WithCpcReq(bool cpcReq) {
            this.cpcReq = cpcReq;
            return this;
        }
        public CreditChargeBuilder WithAllowDuplicates(bool allowDuplicates) {
            this.allowDuplicates = allowDuplicates;
            return this;
        }
        public CreditChargeBuilder WithCardPresent(bool cardPresent) {
            this.cardPresent = cardPresent;
            return this;
        }
        public CreditChargeBuilder WithReaderPresent(bool readerPresent) {
            this.readerPresent = readerPresent;
            return this;
        }
        public CreditChargeBuilder WithGratuity(decimal? gratuity) {
            this.gratuity = gratuity;
            return this;
        }
        public CreditChargeBuilder WithShippingAmt(decimal? shippingAmt)
        {
            this.shippingAmt = shippingAmt;
            return this;
        }
        public CreditChargeBuilder WithConvenienceAmt(decimal? convenienceAmt)
        {
            this.convenienceAmt = convenienceAmt;
            return this;
        }
        public CreditChargeBuilder WithOriginalTxnReferenceData(HpsTxnReferenceData originalTxnReferenceData) {
            this.originalTxnReferenceData = originalTxnReferenceData;
            return this;
        }
        public CreditChargeBuilder WithDirectMarketData(HpsDirectMarketData directMarketData) {
            this.directMarketData = directMarketData;
            return this;
        }
        public CreditChargeBuilder WithAutoSubstantiation(HpsAutoSubstantiation autoSubstantiation) {
            this.autoSubstantiation = autoSubstantiation;
            return this;
        }
        public CreditChargeBuilder WithRewards(HpsGiftCard card) {
            this.rewards = card;
            return this;
        }
        public CreditChargeBuilder WithRewards(string cardNumber) {
            this.rewards = new HpsGiftCard {
                Value = cardNumber,
                ValueType = ItemChoiceType.CardNbr
            };
            return this;
        }
        public CreditChargeBuilder WithSecureEcommerce(HpsSecureEcommerce secureEcommerce)
        {
            this.secureEcommerce = secureEcommerce;
            return this;
        }

        public CreditChargeBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsCharge Execute() {
            base.Execute();

            var block1 = new CreditSaleReqBlock1Type {
                AllowDup = allowDuplicates ? booleanType.Y : booleanType.N,
                AllowDupSpecified = true,
                AllowPartialAuth = allowPartialAuth ? booleanType.Y : booleanType.N,
                AllowPartialAuthSpecified = true,
                Amt = amount.Value
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
            if (token != null)
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

            if (emvData != null)
                block1.EMVData = service.HydrateEmvData(emvData);

            if (secureEcommerce != null)
                block1.SecureECommerce = service.HydrateSecureEcommerce(secureEcommerce);

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCreditSaleReqType{
                    Block1 = block1
                },
                ItemElementName = ItemChoiceType1.CreditSale
            };

            var clientTransactionId = service.GetClientTransactionId(details);
            var response = service.SubmitTransaction(transaction, clientTransactionId);
            var charge = new HpsCharge().FromResponse(response);
            if (rewards != null && charge.ResponseCode == "00") {
                var giftClient = new HpsFluentGiftCardService(service.ServicesConfig);
                try {
                    charge.RewardsResponse = giftClient.Reward(amount).WithCard(rewards).Execute();
                }
                catch (HpsException) { /* NOM NOM */ }
            }

            return charge;
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
            if (card != null) count++;
            if (trackData != null) count++;
            if (token != null) count++;

            return count == 1;
        }
    }
}