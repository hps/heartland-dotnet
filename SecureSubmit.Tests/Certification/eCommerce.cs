using System;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Batch;
using SecureSubmit.Services.Credit;
using SecureSubmit.Services.GiftCard;

namespace SecureSubmit.Tests.Certification
{
    [TestClass]
    public class ECommerce
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };

        private readonly HpsBatchService _batchService = new HpsBatchService(ServicesConfig);
        private readonly HpsCreditService _creditService = new HpsCreditService(ServicesConfig);
        private readonly HpsGiftCardService _giftService = new HpsGiftCardService(ServicesConfig);
        private const bool UseTokens = true;

        [TestMethod]
        public void test_000_CloseBatch()
        {
            try
            {
                var response = _batchService.Close().Execute();
                Assert.IsNotNull(response);
                Console.WriteLine(@"Batch ID: {0}", response.Id);
                Console.WriteLine(@"Sequence Number: {0}", response.SequenceNumber);
            }
            catch (HpsGatewayException e)
            {
                if (e.Code != HpsExceptionCodes.NoOpenBatch && e.Code != HpsExceptionCodes.UnknownIssuerError)
                    Assert.Fail("Something failed other than 'no open batch'.");
                else
                    Console.WriteLine(@"No batch open to close.");
            }
        }

        [TestMethod]
        public void test_001_verify_visa()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Verify()
                .WithCard(TestData.VisaCard(false))
                .RequestMultiuseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void test_002_verify_master_card()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Verify()
                .WithCard(TestData.MasterCard(false))
                .RequestMultiuseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void test_003_verify_discover()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Verify()
                .WithCard(TestData.DiscoverCard(false))
                .WithCardHolder(new HpsCardHolder {Address = new HpsAddress {Zip = "75024"}})
                .RequestMultiuseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        // Address Verification

        [TestMethod]
        public void test_004_verify_amex()
        {
            var cardHolder = new HpsCardHolder {Address = new HpsAddress {Zip = "75024"}};

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Verify()
                    .WithCard(TestData.AmexCard(false))
                    .WithCardHolder(cardHolder)
                    .RequestMultiuseToken(UseTokens)
                    .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Balance Inquiry (for Prepaid Card)

        [TestMethod]
        public void test_005_balance_inquiry_visa()
        {
            var response = _creditService.PrePaidBalanceInquiry()
                .WithCard(TestData.VisaCard(false))
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE (For Multi-Use Token Only)

        [TestMethod]
        public void test_006_charge_visa_token()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Charge(13.01m)
                .WithCard(TestData.VisaCard(false))
                .WithCardHolder(cardHolder)
                .RequestMultiuseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_007_charge_master_card_token()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Charge(13.02m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .RequestMultiuseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_008_charge_discover_token()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Charge(13.03m)
                .WithCard(TestData.DiscoverCard(true))
                .WithCardHolder(cardHolder)
                .RequestMultiuseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_009_charge_amex_token()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };

            // ReSharper disable once RedundantArgumentDefaultValue
            var response = _creditService.Charge(13.04m)
                .WithCard(TestData.AmexCard(true))
                .WithCardHolder(cardHolder)
                .RequestMultiuseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE

        [TestMethod]
        public void test_010_charge_visa()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.01m);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(TestData.VisaMultiUseToken(TestData.Industry.ECommerce)) :
                typeBuilder.WithCard(TestData.VisaCard(true));

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);

            // TEST 35 ONLINE VOID
            var voidResponse = _creditService.Void(chargeResponse.TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void test_011_charge_master_card()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.02m);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(TestData.MasterCardMultiUseToken(TestData.Industry.ECommerce)) :
                typeBuilder.WithCard(TestData.MasterCard(true));

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void test_012_charge_discover()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.03m);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(TestData.DiscoverMultiUseToken(TestData.Industry.ECommerce)) :
                typeBuilder.WithCard(TestData.DiscoverCard(true));

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void test_013_charge_amex()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.04m);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(TestData.AmexMultiUseToken(TestData.Industry.ECommerce)) :
                typeBuilder.WithCard(TestData.AmexCard(true));

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void test_014_charge_jcb()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var chargeResponse = _creditService.Charge(17.05m)
                .WithCard(TestData.JcbCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // AUTHORIZATION

        [TestMethod]
        public void test_015_authorization_visa()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var authResponse = _creditService.Authorize(17.06m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // test 015b Capture/AddToBatch
            var captureResponse = _creditService.Capture(authResponse.TransactionId).Execute();

            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_016_authorization_master_card()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // test 016b Capture/AddToBatch
            var captureResponse = _creditService.Capture(authResponse.TransactionId).Execute();

            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_017_authorization_discover()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(TestData.DiscoverCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);
        }

        // PARTIALLY - APPROVED SALE

        [TestMethod]
        public void test_018_partial_approval_visa()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var response = _creditService.Charge(130m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .AllowPartialAuth()
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void test_019_partial_approval_discover()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var response = _creditService.Charge(145m)
                .WithCard(TestData.DiscoverCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .AllowPartialAuth()
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(65.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void test_020_partial_approval_master_card()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var chargeResponse = _creditService.Charge(155m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .AllowPartialAuth()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("10", chargeResponse.ResponseCode);

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual(100.00m, chargeResponse.AuthorizedAmount);

            // TEST 36 ONLINE VOID
            var voidResponse = _creditService.Void(chargeResponse.TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        // LEVEL II CORPORATE PURCHASE CARD

        [TestMethod]
        public void test_021_level_ii_response_b()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };

            var chargeResponse = _creditService.Charge(112.34m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("B", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData {CardHolderPoNumber = "9876543210", TaxType = taxTypeType.NOTUSED};

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_022_level_ii_response_b()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var chargeResponse = _creditService.Charge(112.34m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .AllowDuplicates()
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("B", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { TaxType = taxTypeType.SALESTAX, TaxAmount = 1.00m };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_023_level_ii_response_r()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(123.45m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("R", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { TaxType = taxTypeType.TAXEXEMPT };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_024_level_ii_response_s()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(134.56m)
                .WithCard(TestData.VisaCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.SALESTAX, TaxAmount = 1.00m };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_025_level_ii_response_s()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.06m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.NOTUSED };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_026_level_ii_response_s()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.07m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { TaxAmount = 1.00m, TaxType = taxTypeType.SALESTAX };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_027_level_ii_response_s()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.08m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxAmount = 1.00m, TaxType = taxTypeType.SALESTAX };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_028_level_ii_response_s()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.09m)
                .WithCard(TestData.MasterCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.TAXEXEMPT };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_029_level_ii_no_response()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.10m)
                .WithCard(TestData.AmexCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.NOTUSED };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_030_level_ii_no_response()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var chargeResponse = _creditService.Charge(111.11m)
                .WithCard(TestData.AmexCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { TaxAmount = 1.00m, TaxType = taxTypeType.SALESTAX };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_031_level_ii_no_response()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var chargeResponse = _creditService.Charge(111.12m)
                .WithCard(TestData.AmexCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxAmount = 1.00m, TaxType = taxTypeType.SALESTAX };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void test_032_level_ii_no_response()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(111.13m)
                .WithCard(TestData.AmexCard(true))
                .WithCardHolder(cardHolder)
                .WithCpcReq()
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.TAXEXEMPT };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        // PRIOR / VOICE AUTHORIZATION

        [TestMethod]
        public void test_033_offline_sale()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var response = _creditService.OfflineCharge(17.10m)
                .WithCard(TestData.VisaCard(true))
                .WithOfflineAuthCode("654321")
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_033_offline_authorization()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var response = _creditService.OfflineAuth(17.10m)
                .WithCard(TestData.VisaCard(true))
                .WithOfflineAuthCode("654321")
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void test_034_offline_credit_return()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var response = _creditService.Refund(15.15m)
                .WithCard(TestData.MasterCard(true))
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ONLINE VOID / REVERSAL

        // test_035_void_test_10: SEE TEST 10
        // test_036_void_test_20: SEE TEST 20

        // ONE CARD - GSB CARD FUNCTIONS

        // BALANCE INQUIRY

        [TestMethod]
        public void test_037_balance_inquiry_gsb()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var response = _creditService.PrePaidBalanceInquiry()
                .WithCard(TestData.GsbCardECommerce())
                .WithCardHolder(cardHolder)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_038_add_value_gsb()
        {
            var trackData = new HpsTrackData
            {
                Value = "%B6277220330000248^ TEST CARD^49121010000000000694?;6277220330000248=49121010000000000694?"
            };

            var response = _creditService.PrePaidAddValue(15.00m)
                .WithTrackData(trackData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // SALE

        [TestMethod]
        public void test_039_charge_gsb()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(2.05m)
                .WithCard(TestData.GsbCardECommerce())
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(new HpsDirectMarketData { InvoiceNumber = "123456" })
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);

            // VOID TRANSACTION

            var voidResponse = _creditService.Void(chargeResponse.TransactionId)
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void test_040_charge_gsb()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var chargeResponse = _creditService.Charge(2.10m)
                .WithCard(TestData.GsbCardECommerce())
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(new HpsDirectMarketData { InvoiceNumber = "123456" })
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // ONLINE VOID / REVERSAL

        // test_041_void_gsb: SEE TEST 39

        // HMS GIFT - REWARDS

        // ACTIVATE

        [TestMethod]
        public void test_042_activate_gift_1()
        {
            var response = _giftService.Activate(6.00m, TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_043_activate_gift_2()
        {
            var response = _giftService.Activate(7.00m, TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // LOAD / ADD VALUE

        [TestMethod]
        public void test_044_add_value_gift_1()
        {
            var response = _giftService.Activate(8.00m, TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_045_add_value_gift_2()
        {
            var response = _giftService.Activate(8.00m, TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // BALANCE INQUIRY

        [TestMethod]
        public void test_046_balance_inquiry_gift_1()
        {
            var response = _giftService.Balance(TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void test_047_balance_inquiry_gift_2()
        {
            var response = _giftService.Balance(TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void test_048_replace_gift_1()
        {
            var response = _giftService.Replace(TestData.GiftCard1(), TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void test_049_replace_gift_2()
        {
            var response = _giftService.Replace(TestData.GiftCard2(), TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // SALE / REDEEM

        [TestMethod]
        public void test_050_sale_gift_1()
        {
            var response = _giftService.Sale(1.0m, TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_051_sale_gift_2()
        {
            var response = _giftService.Sale(2.0m, TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_052_sale_gift_1_void()
        {
            var saleResponse = _giftService.Sale(3.0m, TestData.GiftCard1()).Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("0", saleResponse.ResponseCode);

            // VOID TRANSACTION
            var voidResponse = _giftService.Void(saleResponse.TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("0", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void test_053_sale_gift_2_reversal()
        {
            var saleResponse = _giftService.Sale(4.0m, TestData.GiftCard2()).Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("0", saleResponse.ResponseCode);

            // REVERSE TRANSACTION
            var reverseResponse = _giftService.Reverse(4.0m).UsingTransactionId(saleResponse.TransactionId).Execute();

            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("0", reverseResponse.ResponseCode);
        }

        // VOID

        // test_054_void_gift: SEE TEST 52

        // REVERSAL

        // test_055_reversal_gift: SEE TEST 53

        [TestMethod]
        public void test_056_reversal_gift_2()
        {
            var response = _giftService.Reverse(2.0m)
                .UsingCard(TestData.GiftCard2())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void test_057_deactivate_gift_1()
        {
            var response = _giftService.Deactivate(TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // test_058_receipts_messaging: print and scan receipt for test 51

        // BALANCE INQUIRY

        [TestMethod]
        public void test_059_balance_inquiry_rewards_1()
        {
            var response = _giftService.Balance(TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        [TestMethod]
        public void test_060_balance_inquiry_rewards_2()
        {
            var response = _giftService.Balance(TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        // ALIAS

        [TestMethod]
        public void test_061_create_alias_gift_1()
        {
            var response = _giftService.Alias(HpsGiftCardAliasAction.Create, "9725550100").Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_062_create_alias_gift_2()
        {
            var response = _giftService.Alias(HpsGiftCardAliasAction.Create, "9725550100").Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_063_add_alias_gift_1()
        {
            var response = _giftService.Alias(HpsGiftCardAliasAction.Add, "2145550199")
                .WithGiftCard(TestData.GiftCard1())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_064_add_alias_gift_2()
        {
            var response = _giftService.Alias(HpsGiftCardAliasAction.Add, "2145550199")
                .WithGiftCard(TestData.GiftCard2())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_065_delete_alias_gift_1()
        {
            var response = _giftService.Alias(HpsGiftCardAliasAction.Delete, "2145550199")
                .WithGiftCard(TestData.GiftCard1())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // SALE / REDEEM

        [TestMethod]
        public void test_066_redeem_points_gift_1()
        {
            var response = _giftService.Sale(100.00m, TestData.GiftCard1())
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_067_redeem_points_gift_2()
        {
            var response = _giftService.Sale(200.00m, TestData.GiftCard2())
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_068_redeem_points_gift_2()
        {
            var giftCard = new HpsGiftCard { Number = "9725550100", NumberType = ItemChoiceType.Alias };

            var response = _giftService.Sale(300.00m, giftCard)
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // REWARDS

        [TestMethod]
        public void test_069_rewards_gift_1()
        {
            var response = _giftService.Reward(10.00m, TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_070_rewards_gift_2()
        {
            var response = _giftService.Reward(11.00m, TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void test_071_replace_gift_1()
        {
            var response = _giftService.Replace(TestData.GiftCard1(), TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_072_replace_gift_2()
        {
            var response = _giftService.Replace(TestData.GiftCard2(), TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void test_073_deactivate_gift_1()
        {
            var response = _giftService.Deactivate(TestData.GiftCard1()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_074_deactivate_gift_2()
        {
            var response = _giftService.Deactivate(TestData.GiftCard2()).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // test_075_receipts_messaging: print and scan receipt for test 51

        // CLOSE BATCH

        [TestMethod]
        public void test_999_CloseBatch()
        {
            try
            {
                var response = _batchService.Close().Execute();
                Assert.IsNotNull(response);
                Console.WriteLine(@"Batch ID: {0}", response.Id);
                Console.WriteLine(@"Sequence Number: {0}", response.SequenceNumber);
            }
            catch (HpsGatewayException e)
            {
                if (e.Code != HpsExceptionCodes.NoOpenBatch && e.Code != HpsExceptionCodes.UnknownIssuerError)
                    Assert.Fail("Something failed other than 'no open batch'.");
                else
                    Console.WriteLine(@"No batch open to close.");
            }
        }
    }
}
