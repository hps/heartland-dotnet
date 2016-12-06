using System.Net;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Services {
    /// <summary>The HPS service.</summary>
    public abstract class HpsSoapGatewayService {
        bool enableLogging = false;

        protected HpsSoapGatewayService(IHpsServicesConfig config = null, bool enableLogging = false) 
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicesConfig = config;
            this.enableLogging = enableLogging;
        }

        internal IHpsServicesConfig ServicesConfig { get; set; }

        internal PosResponse DoTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null) {
            // Check for a valid config.
            if (IsConfigInvalid()) {
                throw new HpsAuthenticationException(HpsExceptionCodes.InvalidConfiguration, Resource.Exception_Message_InvalidConfig);
            }

            var req = new PosRequest {
                Ver10 = new PosRequestVer10 {
                    Header = new PosRequestVer10Header {
                        SecretAPIKey = ServicesConfig.SecretApiKey,
                        LicenseId = ServicesConfig.LicenseId,
                        SiteId = ServicesConfig.SiteId,
                        DeviceId = ServicesConfig.DeviceId,
                        VersionNbr = ServicesConfig.VersionNumber,
                        UserName = ServicesConfig.UserName,
                        Password = ServicesConfig.Password,
                        DeveloperID = ServicesConfig.DeveloperId,
                        SiteTrace = ServicesConfig.SiteTrace
                    },
                    Transaction = transaction
                }
            };

            if (clientTransactionId.HasValue) {
                req.Ver10.Header.ClientTxnId = clientTransactionId.Value;
                req.Ver10.Header.ClientTxnIdSpecified = true;
            }

            using (var client = new PosGatewayService { Url = ServicesConfig.ServiceUrl }) {
                return client.DoTransaction(req);
            }
        }

        private bool IsConfigInvalid() {
            return ServicesConfig == null 
                && ServicesConfig.SecretApiKey == null 
                && (ServicesConfig.LicenseId == default(int) 
                    || ServicesConfig.SiteId == default(int) 
                    || ServicesConfig.DeviceId == default(int)
                    || ServicesConfig.UserName == null
                    || ServicesConfig.Password == null);
        }

        internal HpsTransactionHeader HydrateTransactionHeader(PosResponseVer10Header header) {
            var hpsTransactionHeader = new HpsTransactionHeader {
                GatewayRspMsg = header.GatewayRspMsg,
                GatewayRspCode = header.GatewayRspCode,
                RspDt = header.RspDT
            };

            return hpsTransactionHeader;
        }

        internal long? GetClientTransactionId(PosResponseVer10Header header) {
            if (header.ClientTxnIdSpecified) { return header.ClientTxnId; }
            return null;
        }

        internal long? GetClientTransactionId(HpsTransactionDetails details) {
            if (details != null)
                return details.ClientTransactionId;
            return null;
        }

        internal CardHolderDataType HydrateCardHolderData(HpsConsumer consumer) {
            return new CardHolderDataType {
                CardHolderFirstName = consumer.FirstName,
                CardHolderLastName = consumer.LastName,
                CardHolderEmail = consumer.Email,
                CardHolderPhone = consumer.Phone,
                CardHolderAddr = consumer.Address.Address,
                CardHolderCity = consumer.Address.City,
                CardHolderState = consumer.Address.State,
                CardHolderZip = consumer.Address.Zip,
            };
        }

        internal CardDataTypeManualEntry HydrateCardManualEntry(HpsCreditCard card, bool cardPresent = false, bool readerPresent = false) {
            return new CardDataTypeManualEntry {
                CardNbr = card.Number,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
                CVV2 = card.Cvv,
                CVV2StatusSpecified = false,
                CardPresent = cardPresent ? booleanType.Y : booleanType.N,
                CardPresentSpecified = true,
                ReaderPresent = readerPresent ? booleanType.Y : booleanType.N,
                ReaderPresentSpecified = true,
            };
        }

        internal AdditionalTxnFieldsType HydrateAdditionalTxnFields(HpsTransactionDetails details) {
            return details == null ? null : new AdditionalTxnFieldsType {
                Description = details.Memo,
                CustomerID = details.CustomerId,
                InvoiceNbr = details.InvoiceNumber
            };
        }

        internal EncryptionDataType HydrateEncryptionData(HpsEncryptionData encryptionData) {
            return encryptionData != null ? new EncryptionDataType {
                EncryptedTrackNumber = encryptionData.EncryptedTrackNumber,
                KSN = encryptionData.Ksn,
                KTB = encryptionData.Ktb,
                Version = encryptionData.Version
            } : null;
        }

        internal EMVDataType HydrateEmvData(HpsEmvDataType emvData) {
            return emvData != null ? new EMVDataType {
                EMVTagData = emvData.TagData,
                EMVChipCondition =  emvData.ChipCondition,
                PINBlock = emvData.PinBlock,
                EMVChipConditionSpecified = emvData.ChipConditionSpecified
            } : null;
        }

        internal CardDataTypeTrackData HydrateCardTrackData(HpsTrackData trackData) {
            return trackData != null ? new CardDataTypeTrackData {
                method = trackData.Method == HpsTrackDataMethod.Swipe ? CardDataTypeTrackDataMethod.swipe : CardDataTypeTrackDataMethod.proximity,
                methodSpecified = true,
                Value = trackData.Value
            } : null;
        }

        internal DirectMktDataType HydrateDirectMktData(HpsDirectMarketData directMarketData) {
            return directMarketData != null ? new DirectMktDataType {
                DirectMktInvoiceNbr = directMarketData.InvoiceNumber,
                DirectMktShipDay = directMarketData.ShipDay,
                DirectMktShipMonth = directMarketData.ShipMonth
            } : null;
        }

        internal CPCDataType HydrateCpcData(HpsCpcData cpcData) {
            if (cpcData != null) {
                var cpcElement = new CPCDataType();
                if (cpcData.CardHolderPoNumber != null)
                    cpcElement.CardHolderPONbr = cpcData.CardHolderPoNumber;

                cpcElement.TaxAmtSpecified = cpcData.TaxAmount.HasValue;
                if (cpcElement.TaxAmtSpecified)
                    cpcElement.TaxAmt = cpcData.TaxAmount.Value;

                cpcElement.TaxTypeSpecified = cpcData.TaxType.HasValue;
                if (cpcElement.TaxTypeSpecified)
                    cpcElement.TaxType = cpcData.TaxType.Value;

                return cpcElement;
            }
            return null;
        }

        internal CardDataTypeTokenData HydrateTokenData(string token, bool cardPresent = false, bool readerPresent = false)
        {
            return HydrateTokenData(
                new HpsTokenData { TokenValue = token },
                cardPresent,
                readerPresent
            );
        }

        internal CardDataTypeTokenData HydrateTokenData(HpsTokenData token, bool cardPresent = false, bool readerPresent = false)
        {
            var cardDataTypeTokenData = new CardDataTypeTokenData {
                TokenValue = token.TokenValue,
                CardPresent = cardPresent ? booleanType.Y : booleanType.N,
                CardPresentSpecified = true,
                ReaderPresent = readerPresent ? booleanType.Y : booleanType.N,
                ReaderPresentSpecified = true,
                ExpMonth = token.ExpMonth.HasValue ? token.ExpMonth.Value : default(int),
                ExpMonthSpecified = token.ExpMonth.HasValue,
                ExpYear = token.ExpYear.HasValue ? token.ExpYear.Value : default(int),
                ExpYearSpecified = token.ExpYear.HasValue
            };

            if (!string.IsNullOrEmpty(token.CVV))
            {
                cardDataTypeTokenData.CVV2 = token.CVV;
            }

            return cardDataTypeTokenData;
        }

        internal AutoSubstantiationType HydrateAutoSubstantiation(HpsAutoSubstantiation autoSubstantiation) {
            if (autoSubstantiation != null) {
                var autoElement = new AutoSubstantiationType();
                if (autoSubstantiation.MerchantVerificationValue != null)
                    autoElement.MerchantVerificationValue = autoSubstantiation.MerchantVerificationValue;
                autoElement.RealTimeSubstantiation = autoSubstantiation.RealTimeSubstantiation ? booleanType.Y : booleanType.N;

                string[] amountCount = new string[] { "First", "Second", "Third", "Fourth" };
                HpsAdditionalAmount[] values = autoSubstantiation.AdditionalAmounts;
                for (int i = 0; i < values.Length; i++) {
                    HpsAdditionalAmount value = values[i];

                    AdditionalAmtType amount = new AdditionalAmtType {
                        Amt = value.Amount,
                        AmtType = value.AmountType
                    };

                    var field = autoElement.GetType().GetField(amountCount[i] + "AdditionalAmtInfo");
                    field.SetValue(autoElement, amount);
                }

                return autoElement;
            }
            return null;
        }

        internal GiftCardDataType HydrateGiftCardData(HpsGiftCard card, string elementName = "CardData") {
            GiftCardDataType cardElement = new GiftCardDataType {
                Item = card.Value,
                ItemElementName = card.ValueType,
                EncryptionData = HydrateEncryptionData(card.EncryptionData),
                PIN = card.Pin
            };

            return cardElement;
        }

        internal ConsumerInfoType HydrateConsumerInfo(HpsCheckHolder checkHolder) {
            var consumerInfo = new ConsumerInfoType();

            if (checkHolder.Address != null) {
                HpsAddress address = checkHolder.Address;

                consumerInfo.Address1 = address.Address;
                consumerInfo.City = address.City;
                consumerInfo.State = address.State;
                consumerInfo.Zip = address.Zip;
            }

            consumerInfo.CheckName = checkHolder.CheckName;
            consumerInfo.CourtesyCard = checkHolder.CourtesyCard;
            consumerInfo.DLNumber = checkHolder.DlNumber;
            consumerInfo.DLState = checkHolder.DlState;
            consumerInfo.EmailAddress = checkHolder.Email;
            consumerInfo.FirstName = checkHolder.FirstName;
            consumerInfo.LastName = checkHolder.LastName;
            consumerInfo.PhoneNumber = checkHolder.Phone;

            if (checkHolder.Ssl4 != null || checkHolder.DobYear != null) {
                consumerInfo.IdentityInfo = new IdentityInfoType();
                
                if(checkHolder.DobYear.HasValue)
                    consumerInfo.IdentityInfo.DOBYear = checkHolder.DobYear.Value.ToString();
                if(!string.IsNullOrEmpty(checkHolder.Ssl4))
                    consumerInfo.IdentityInfo.SSNL4 = checkHolder.Ssl4;
            }

            return consumerInfo;
        }

        internal AccountInfoType HydrateCheckData(HpsCheck check) {
            var accountInfo = new AccountInfoType();
            accountInfo.AccountNumber = check.AccountNumber;
            accountInfo.CheckNumber = check.CheckNumber;
            accountInfo.MICRData = check.MicrNumber;
            accountInfo.RoutingNumber = check.RoutingNumber;
            if (check.AccountType.HasValue) {
                accountInfo.AccountType = check.AccountType.Value;
                accountInfo.AccountTypeSpecified = true;
            }

            return accountInfo;
        }

        protected TAuthorization HydrateAuthorization<TAuthorization>(PosResponseVer10 response)
            where TAuthorization : HpsAuthorization, new() {
            var authRsp = (AuthRspStatusType)response.Transaction.Item;

            return new TAuthorization {
                Header = HydrateTransactionHeader(response.Header),
                TransactionId = response.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(response.Header),
                AvsResultCode = authRsp.AVSRsltCode,
                AvsResultText = authRsp.AVSRsltText,
                CvvResultCode = authRsp.CVVRsltCode,
                CvvResultText = authRsp.CVVRsltText,
                Descriptor = authRsp.TxnDescriptor,
                AuthorizationCode = authRsp.AuthCode,
                AuthorizedAmount = authRsp.AuthAmt,
                ReferenceNumber = authRsp.RefNbr,
                ResponseCode = authRsp.RspCode,
                ResponseText = authRsp.RspText,
                CardType = authRsp.CardType,
                CpcIndicator = authRsp.CPCInd
            };
        }

        internal SecureECommerceType HydrateSecureEcommerce(HpsSecureEcommerce data)
        {
            var secureEcommerce = new SecureECommerceType
            {
                ECommerceIndicator = data.EciFlag,
                PaymentData = new SecureECommerceTypePaymentData
                {
                    encoding = EncodingType.base64,
                    Value = data.Data
                },
                XID = new SecureECommerceTypeXID
                {
                    encoding = EncodingType.base64,
                    Value = data.Xid
                },
                TypeOfPaymentData = TypeOfPaymentDataType.Item3DSecure
            };
            return secureEcommerce;
        }
    }
}