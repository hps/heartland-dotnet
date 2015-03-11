// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGatewayService.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsGatewayService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Services
{
    /// <summary>The HPS service.</summary>
    public abstract class HpsGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsGatewayService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        protected HpsGatewayService(IHpsServicesConfig config = null)
        {
            ServicesConfig = config;
        }

        /// <summary>Gets or sets the HPS services config.</summary>
        private IHpsServicesConfig ServicesConfig { get; set; }

        /// <summary>The do transaction.</summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="clientTransactionId">An optional client transaction ID.</param>
        /// <returns>The <see cref="PosResponse"/>.</returns>
        protected PosResponse DoTransaction(PosRequestVer10Transaction transaction, long? clientTransactionId = null)
        {
            // Check for a valid config.
            if (IsConfigInvalid())
            {
                throw new HpsAuthenticationException(HpsExceptionCodes.InvalidConfiguration, Resource.Exception_Message_InvalidConfig);
            }

            var req = new PosRequest
            {
                Ver10 = new PosRequestVer10
                {
                    Header = new PosRequestVer10Header
                    {
                        SecretAPIKey = (ServicesConfig == null) ? HpsConfiguration.SecretApiKey : ServicesConfig.SecretApiKey,
                        LicenseId = (ServicesConfig == null) ? HpsConfiguration.LicenseId : ServicesConfig.LicenseId,
                        SiteId = (ServicesConfig == null) ? HpsConfiguration.SiteId : ServicesConfig.SiteId,
                        DeviceId = (ServicesConfig == null) ? HpsConfiguration.DeviceId : ServicesConfig.DeviceId,
                        VersionNbr = (ServicesConfig == null) ? HpsConfiguration.VersionNumber : ServicesConfig.VersionNumber,
                        UserName = (ServicesConfig == null) ? HpsConfiguration.UserName : ServicesConfig.UserName,
                        Password = (ServicesConfig == null) ? HpsConfiguration.Password : ServicesConfig.Password,
                        DeveloperID = (ServicesConfig == null) ? HpsConfiguration.DeveloperId : ServicesConfig.DeveloperId,
                        SiteTrace = (ServicesConfig == null) ? HpsConfiguration.SiteTrace : ServicesConfig.SiteTrace
                    },
                    Transaction = transaction
                }
            };

            if (clientTransactionId.HasValue)
            {
                req.Ver10.Header.ClientTxnId = clientTransactionId.Value;
                req.Ver10.Header.ClientTxnIdSpecified = true;
            }

            using (var client = new PosGatewayService { Url = HpsConfiguration.SoapServiceUri })
            {
                return client.DoTransaction(req);
            }
        }

        /// <summary>Hydrate a new HPS transaction response header.</summary>
        /// <param name="header">The response header from the HPS gateway.</param>
        /// <returns>The <see cref="HpsTransactionHeader"/>.</returns>
        internal HpsTransactionHeader HydrateTransactionHeader(PosResponseVer10Header header)
        {
            var hpsTransactionHeader = new HpsTransactionHeader
            {
                GatewayRspMsg = header.GatewayRspMsg,
                GatewayRspCode = header.GatewayRspCode,
                RspDt = header.RspDT
            };

            return hpsTransactionHeader;
        }

        /// <summary>Determine whether the HPS config has been initialized, in one way or another.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool IsConfigInvalid()
        {
            return ServicesConfig == null
                   && (HpsConfiguration.SecretApiKey == null 
                       || HpsConfiguration.LicenseId == -1 || HpsConfiguration.DeviceId == -1
                       || HpsConfiguration.Password == null || HpsConfiguration.SoapServiceUri == null
                       || HpsConfiguration.SiteId == -1 || HpsConfiguration.UserName == null);
        }

        protected long? GetClientTransactionId(PosResponseVer10Header header)
        {
            if (header.ClientTxnIdSpecified) { return header.ClientTxnId; }
            return null;
        }

        protected CardHolderDataType HydrateCardHolderData(HpsConsumer consumer)
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
                CardHolderZip = consumer.Address.Zip,
            };
        }

        protected CardDataTypeManualEntry HydrateCardManualEntry(HpsCreditCard card)
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

        protected AdditionalTxnFieldsType HydrateAdditionalTxnFields(HpsTransactionDetails details)
        {
            return details == null ? null : new AdditionalTxnFieldsType
            {
                Description = details.Memo,
                CustomerID = details.CustomerId,
                InvoiceNbr = details.InvoiceNumber
            };
        }

        protected EncryptionDataType HydrateEncryptionData(HpsEncryptionData encryptionData)
        {
            return encryptionData != null ? new EncryptionDataType
            {
                EncryptedTrackNumber = encryptionData.EncryptedTrackNumber,
                KSN = encryptionData.Ksn,
                KTB = encryptionData.Ktb,
                Version = encryptionData.Version
            } : null;
        }

        protected EMVDataType HydrateEmvData(string emvData)
        {
            return string.IsNullOrEmpty(emvData) ? null : new EMVDataType
            {
                EMVTagData = emvData
            };
        }

        protected CardDataTypeTrackData HydrateCardTrackData(HpsTrackData trackData)
        {
            return trackData != null ? new CardDataTypeTrackData
            {
                method = trackData.Mehod == HpsTrackDataMethod.Swipe ? CardDataTypeTrackDataMethod.swipe : CardDataTypeTrackDataMethod.proximity,
                methodSpecified = true,
                Value = trackData.Value
            } : null;
        }

        protected DirectMktDataType HydrateDirectMarketData(HpsDirectMarketData directMarketData)
        {
            return directMarketData != null ? new DirectMktDataType
            {
                DirectMktInvoiceNbr = directMarketData.InvoiceNumber,
                DirectMktShipDay = directMarketData.ShipDay,
                DirectMktShipMonth = directMarketData.ShipMonth
            } : null;
        }

        protected HpsAuthorization HydrateAuthorization(PosResponseVer10 response)
        {
            var authRsp = (AuthRspStatusType)response.Transaction.Item;

            return new HpsAuthorization
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
    }
}