using System.Collections.Generic;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Services.Fluent
{
    public abstract class GatewayTransactionBuilder<TBuilder, TExecutionResult> : TransactionBuilder<TBuilder, TExecutionResult> where TBuilder : class
    {
        protected IHpsServicesConfig ServicesConfig;
        protected internal PosRequestVer10Transaction Transaction;
        protected internal int? ClientTransactionId;

        protected GatewayTransactionBuilder(IHpsServicesConfig config)
        {
            ServicesConfig = config;
        }

        public TBuilder WithClientTransactionId(int clientTransactionId)
        {
            ClientTransactionId = clientTransactionId;
            return this as TBuilder;
        }

        protected internal PosResponse DoTransaction()
        {
            return HpsSoapGatewayService.DoTransaction(ServicesConfig, Transaction, ClientTransactionId);
        }

        protected static long? GetClientTransactionId(PosResponseVer10Header header)
        {
            if (header.ClientTxnIdSpecified) { return header.ClientTxnId; }
            return null;
        }

        protected static HpsTransactionHeader HydrateTransactionHeader(PosResponseVer10Header header)
        {
            var hpsTransactionHeader = new HpsTransactionHeader
            {
                GatewayRspMsg = header.GatewayRspMsg,
                GatewayRspCode = header.GatewayRspCode,
                RspDt = header.RspDT
            };

            return hpsTransactionHeader;
        }

        protected static CardDataTypeManualEntry HydrateCardManualEntry(HpsCreditCard card)
        {
            return new CardDataTypeManualEntry
            {
                CardNbr = card.Number,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
                CVV2 = card.Cvv,
                CVV2StatusSpecified = false,
                CardPresent = booleanType.N,
                CardPresentSpecified = true,
                ReaderPresent = booleanType.N,
                ReaderPresentSpecified = true,
            };
        }

        protected static CardHolderDataType HydrateCardHolderData(HpsConsumer consumer)
        {
            return new CardHolderDataType
            {
                CardHolderFirstName = consumer.FirstName,
                CardHolderLastName = consumer.LastName,
                CardHolderEmail = consumer.Email,
                CardHolderPhone = consumer.Phone,
                CardHolderAddr = consumer.Address.Address,
                CardHolderCity = consumer.Address.City,
                CardHolderState = consumer.Address.State,
                CardHolderZip = consumer.Address.Zip
            };
        }

        protected static AdditionalTxnFieldsType HydrateAdditionalTxnFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            return additionalTransactionFields != null ? new AdditionalTxnFieldsType
            {
                CustomerID = additionalTransactionFields.CustomerId,
                Description = additionalTransactionFields.Description,
                InvoiceNbr = additionalTransactionFields.InvoiceNumber
            } : null;
        }

        protected static EncryptionDataType HydrateEncryptionData(HpsEncryptionData encryptionData)
        {
            return encryptionData != null ? new EncryptionDataType
            {
                EncryptedTrackNumber = encryptionData.EncryptedTrackNumber,
                KSN = encryptionData.Ksn,
                KTB = encryptionData.Ktb,
                Version = encryptionData.Version
            } : null;
        }

        protected DirectMktDataType HydrateDirectMktData(HpsDirectMarketData directMarketData)
        {
            return directMarketData != null ? new DirectMktDataType
            {
                DirectMktInvoiceNbr = directMarketData.InvoiceNumber,
                DirectMktShipDay = directMarketData.ShipDay,
                DirectMktShipMonth = directMarketData.ShipMonth
            } : null;
        }

        protected static TAuthorization HydrateAuthorization<TAuthorization>(PosResponseVer10 response)
            where TAuthorization : HpsAuthorization, new()
        {
            var authRsp = (AuthRspStatusType)response.Transaction.Item;

            return new TAuthorization
            {
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

        protected static CardDataTypeTrackData HydrateCardTrackData(HpsTrackData trackData)
        {
            return trackData != null ? new CardDataTypeTrackData
            {
                method = trackData.Method == HpsTrackDataMethod.Swipe ? CardDataTypeTrackDataMethod.swipe : CardDataTypeTrackDataMethod.proximity,
                methodSpecified = true,
                Value = trackData.Value
            } : null;
        }

        protected static GiftCardDataType HydrateGiftCardData(HpsGiftCard giftCard)
        {
            return new GiftCardDataType
            {
                Item = giftCard.Number,
                ItemElementName = giftCard.IsTrackData ? ItemChoiceType.TrackData : ItemChoiceType.CardNbr,
                EncryptionData = (giftCard.EncryptionData == null) ? null : new EncryptionDataType
                {
                    EncryptedTrackNumber = giftCard.EncryptionData.EncryptedTrackNumber,
                    KSN = giftCard.EncryptionData.Ksn,
                    KTB = giftCard.EncryptionData.Ktb,
                    Version = giftCard.EncryptionData.Version
                }
            };
        }

        protected static List<HpsCheckResponseDetails> HydrateCheckResponseDetails(IEnumerable<CheckRspInfoType> responseInfo)
        {
            var result = new List<HpsCheckResponseDetails>();
            foreach (var info in responseInfo)
            {
                result.Add(new HpsCheckResponseDetails
                {
                    Code = info.Code,
                    FieldName = info.FieldName,
                    FieldNumber = info.FieldNumber,
                    Message = info.Message,
                    MessageType = info.Type
                });
            }

            return result;
        }

        protected static CPCDataType HydrateCpcData(HpsCpcData cpcData)
        {
            var result = new CPCDataType { CardHolderPONbr = cpcData.CardHolderPoNumber };

            if (cpcData.TaxAmount != null)
            {
                result.TaxAmt = cpcData.TaxAmount.Value;
                result.TaxAmtSpecified = true;
            }

            if (cpcData.TaxType == null) return result;
            
            result.TaxType = cpcData.TaxType.Value;
            result.TaxTypeSpecified = true;

            return result;
        }
    }
}
