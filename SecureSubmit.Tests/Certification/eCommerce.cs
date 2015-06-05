using System;
using System.Text.RegularExpressions;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Batch;
using SecureSubmit.Services.Credit;
using SecureSubmit.Services.Fluent.Credit;
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
            var directMarketData = new HpsDirectMarketData {InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1};

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456", ShipMonth = 1, ShipDay = 1 };

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
    }
}
