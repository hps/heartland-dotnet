using System;
using System.Reflection.Emit;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Tests.TestData;


namespace SecureSubmit.Tests
{
    using SecureSubmit.Services;

    [TestClass]
    public class EMVTests
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };

        private readonly HpsBatchService _batchService = new HpsBatchService(ServicesConfig);
        private readonly HpsFluentCreditService _creditService = new HpsFluentCreditService(ServicesConfig);
        private readonly HpsFluentGiftCardService _giftService = new HpsFluentGiftCardService(ServicesConfig);

        [TestMethod]
        public void authorize_visa_EMV()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var emvData = new HpsEmvDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001"
            };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Authorize(25.00m)
                .WithTrackData(trackData)
                .WithEMVData(emvData)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void charge_visa_EMV_Prev_Chip_Card_Read_Failed()
        {
            var cardHolder = new HpsCardHolder {Address = new HpsAddress {Address = "6860 Dallas Pkwy", Zip = "75024"}};
            var directMarketData = new HpsDirectMarketData {InvoiceNumber = "123456"};
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var typeBuilder = _creditService.Charge(17.01m);

            var emvData = new HpsEmvDataType
            {
                ChipCondition = emvChipConditionType.CHIP_FAILED_PREV_FAILED,
                ChipConditionSpecified = false,
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001"
            };

            var builder = typeBuilder.WithTrackData(trackData);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithEMVData(emvData)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void charge_visa_EMV_Prev_Chip_Card_Read_Success()
        {
            var cardHolder = new HpsCardHolder {Address = new HpsAddress {Address = "6860 Dallas Pkwy", Zip = "75024"}};
            var directMarketData = new HpsDirectMarketData {InvoiceNumber = "123456"};
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var typeBuilder = _creditService.Charge(17.02m);

            var emvData = new HpsEmvDataType
            {
                ChipCondition = emvChipConditionType.CHIP_FAILED_PREV_SUCCESS,
                ChipConditionSpecified = false,
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001"
            };

            var builder = typeBuilder.WithTrackData(trackData);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithEMVData(emvData)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void charge_visa_EMV_Issuer_Response_is_Present()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress() { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var emvData = new HpsEmvDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001"
            };

            var typeBuilder = _creditService.Charge(17.03m);
            var builder = typeBuilder.WithTrackData(trackData);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithEMVData(emvData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.IsNotNull(chargeResponse.EMVIssuerResp);
        }

        [TestMethod]
        public void offlinecharge_visa_EMV()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var emvData = new EMVDataType
            {
                EMVTagData = "9F4005F000F0A0019F02060000000001219F03060000000000009F260816AC7EB8C0DFC40982027C005F3401019F360203869F0702FF009F0802008C9F0902008C8A0259319F34031E03009F2701409F0D05F0400088009F0E0500100000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000800005F2A0208409A031409029B02E8009F21031145219C01009F3704BEBD49924F07A00000000310109F0607A00000000310108407A00000000310109F100706010A039000029F410400000001"
            };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.OfflineCharge(25.00m)
                .WithTrackData(trackData)
                .WithEMVData(emvData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        
        [TestMethod]
        public void verify_visa_EMV()
        {
            var trackData = new HpsTrackData { Value = ";4761739001010036=15122011184404889?" };

            var emvData = new HpsEmvDataType
            {
                TagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001"
            };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Verify()
                .WithTrackData(trackData)
                .WithEMVData(emvData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

    }

}
