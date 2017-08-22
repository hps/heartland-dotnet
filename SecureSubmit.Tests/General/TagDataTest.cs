using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSubmit.Tests.General
{
    [TestClass]
    public class TagDataTest
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };

        private readonly HpsBatchService _batchService = new HpsBatchService(ServicesConfig);
        private readonly HpsFluentCreditService _creditService = new HpsFluentCreditService(ServicesConfig);
        private readonly HpsFluentGiftCardService _giftService = new HpsFluentGiftCardService(ServicesConfig);
        #region CreditFluentService 
        [TestMethod]
        public void authorize_visa_tagdata()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };            
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .WithAllowDuplicates(true)                                
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void authorize_tagdata_client_transactionid()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var details = new HpsTransactionDetails
            {
                ClientTransactionId = 1234567890,
                InvoiceNumber = "1234"
            };
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .WithAllowDuplicates(true)
                .WithDetails(details)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ClientTransactionId);
            Assert.AreEqual(1234567890, response.ClientTransactionId);
        }

        [TestMethod]
        public void authorize_tagdata_issuer_response()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var details = new HpsTransactionDetails
            {
                ClientTransactionId = 1234567890,
                InvoiceNumber = "1234"
            };
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .WithAllowDuplicates(true)
                .WithDetails(details)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.EMVIssuerResp);            
        }

        [TestMethod]
        public void authorize_tagdata_reference_number()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var details = new HpsTransactionDetails
            {
                ClientTransactionId = 1234567890,
                InvoiceNumber = "1234"
            };
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .WithAllowDuplicates(true)
                .WithDetails(details)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ReferenceNumber);
        }

        [TestMethod]
        public void authorize_tagdata_response_text()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var details = new HpsTransactionDetails
            {
                ClientTransactionId = 1234567890,
                InvoiceNumber = "1234"
            };
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .WithAllowDuplicates(true)
                .WithDetails(details)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ResponseText);
        }

        [TestMethod]
        public void offlinecharge_chip_approval()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000001219F03060000000000009F260816AC7EB8C0DFC40982027C005F3401019F360203869F0702FF009F0802008C9F0902008C8A0259319F34031E03009F2701409F0D05F0400088009F0E0500100000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000800005F2A0208409A031409029B02E8009F21031145219C01009F3704BEBD49924F07A00000000310109F0607A00000000310108407A00000000310109F100706010A039000029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = _creditService.OfflineCharge(25.00m)
                .WithTrackData(trackData)
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsGatewayException))]
        public void offlinecharge_chip_decline()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000016009F03060000000000009F2608C71803025DEC760E820258005F3401019F360200019F0702FF009F080200019F090200018A025A319F34035E03009F2701009F0D05F040C428009F0E0500100000009F0F05F068DCF8005F280208409F390105FFC605DC00002000FFC7050010000000FFC805FCE09CF8009F3303E028C89F1A0208409F350122950500000080005F2A0208409A031410109B02E8009F21030957479C01009F3704FF3DCEA04F07A00000015230109F0607A00000015230108407A00000015230109F10080105A000000020009F410400000003",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = _creditService.OfflineCharge(25.00m)
                .WithTrackData(trackData)
                .WithAllowDuplicates(false)
                .WithTagData(tagData)
                .Execute();
        }

        [TestMethod]
        public void verify_visa_tagdata()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = _creditService.Verify()
                .WithTrackData(trackData)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void charge_visa_tag_Issuer_Response()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress() { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var typeBuilder = _creditService.Charge(17.03m);
            var builder = typeBuilder.WithTrackData(trackData);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.IsNotNull(chargeResponse.EMVIssuerResp);
        }
        #endregion
        #region CreditService
        [TestMethod]
        public void CreditAuthWithTagData()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = creditService.Authorize(10m, "usd", trackData, null, 0, false, false, null, tagData, 0, 0);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditChargeWithTagData()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = creditService.Charge(10m, "usd", trackData, null, 0, false, false, null, tagData, false, 0, 0);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditOfflineChargeWithTagData()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000001219F03060000000000009F260816AC7EB8C0DFC40982027C005F3401019F360203869F0702FF009F0802008C9F0902008C8A0259319F34031E03009F2701409F0D05F0400088009F0E0500100000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000800005F2A0208409A031409029B02E8009F21031145219C01009F3704BEBD49924F07A00000000310109F0607A00000000310108407A00000000310109F100706010A039000029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = creditService.OfflineCharge(10m, "usd", trackData, null, 0, 0, null, tagData, 0, 0);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerifyWithTagData()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };
            var tagData = new HpsTagDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001",
                Source = TagDataTypeTagValuesSource.chip
            };
            var response = creditService.Verify(trackData, null, false, null, tagData);
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }
        #endregion
    }
}
