using System;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;

namespace SecureSubmit.Tests.Certification
{
    [TestClass]
    public class MOTO
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MRCQAQBC_VQACBE0rFaZlbDDPieMGP06JDAtjyS7NQ"
        };

        private readonly HpsBatchService _batchService = new HpsBatchService(ServicesConfig);
        private readonly HpsFluentCreditService _creditService = new HpsFluentCreditService(ServicesConfig);
        private readonly HpsFluentGiftCardService _giftService = new HpsFluentGiftCardService(ServicesConfig);
        private const bool UseTokens = false;

        private static string visaToken;
        private static string masterCardToken;
        private static string discoverToken;
        private static string amexToken;

        private static long test10TransactionId;
        private static long test20TransactionId;
        private static long test39TransactionId;
        private static long test52TransactionId;
        private static long test53TransactionId;

        [TestMethod]
        public void moto_000_CloseBatch()
        {
            try
            {
                var response = _batchService.CloseBatch();
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
        public void moto_001_verify_visa() {
            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.Verify()
                .WithCard(card)
                .WithRequestMultiUseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void moto_002_verify_master_card() {
            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.Verify()
                .WithCard(card)
                .WithRequestMultiUseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void moto_003_verify_discover() {
            var card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.Verify()
                .WithCard(card)
                .WithCardHolder(new HpsCardHolder { Address = new HpsAddress { Zip = "75024" } })
                .WithRequestMultiUseToken(UseTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        // Address Verification

        [TestMethod]
        public void moto_004_verify_amex() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.Verify()
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithRequestMultiUseToken(UseTokens)
                    .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Balance Inquiry (for Prepaid Card)

        [TestMethod]
        public void moto_005_balance_inquiry_visa() {
            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.PrePaidBalanceInquiry()
                .WithCard(card)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE (For Multi-Use Token Only)

        [TestMethod]
        public void moto_006_charge_visa_token() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = _creditService.Charge(13.01m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            visaToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void moto_007_charge_master_card_token() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            
            var response = _creditService.Charge(13.02m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            masterCardToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void moto_008_charge_discover_token() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            
            var response = _creditService.Charge(13.03m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            discoverToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void moto_009_charge_amex_token() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            
            var response = _creditService.Charge(13.04m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            amexToken = response.TokenData.TokenValue;
        }

        // CREDIT SALE

        [TestMethod]
        public void moto_010_charge_visa() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.01m);

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(visaToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            test10TransactionId = chargeResponse.TransactionId;
        }

        [TestMethod]
        public void moto_011_charge_master_card() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.02m);

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(masterCardToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void moto_012_charge_discover() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.03m);

            var card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(discoverToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void moto_013_charge_amex() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.04m);

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            // ReSharper disable once CSharpWarnings::CS0162
            var builder = UseTokens ? typeBuilder.WithToken(amexToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void moto_014_charge_jcb() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(17.05m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // AUTHORIZATION

        [TestMethod]
        public void moto_015_authorization_visa() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.06m)
                .WithCard(card)
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
        public void moto_016_authorization_master_card() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(card)
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
        public void moto_017_authorization_discover() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);
        }

        // PARTIALLY - APPROVED SALE

        [TestMethod]
        public void moto_018_partial_approval_visa() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.Charge(130m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void moto_019_partial_approval_discover() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.Charge(145m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(65.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void moto_020_partial_approval_master_card() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(155m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("10", chargeResponse.ResponseCode);

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual(100.00m, chargeResponse.AuthorizedAmount);
            test20TransactionId = chargeResponse.TransactionId;
        }

        // LEVEL II CORPORATE PURCHASE CARD

        [TestMethod]
        public void moto_021_level_ii_response_b() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(112.34m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithAllowDuplicates(true)
                .WithCpcReq(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("B", chargeResponse.CpcIndicator);

            var cpcData = new HpsCpcData { CardHolderPoNumber = "9876543210", TaxType = taxTypeType.NOTUSED };

            var cpcResponse = _creditService.CpcEdit(chargeResponse.TransactionId)
                .WithCpcData(cpcData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void moto_022_level_ii_response_b() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(112.34m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithAllowDuplicates(true)
                .WithCpcReq(true)
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
        public void moto_023_level_ii_response_r() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(123.45m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_024_level_ii_response_s() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(134.56m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_025_level_ii_response_s() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(111.06m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_026_level_ii_response_s() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(111.07m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_027_level_ii_response_s() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(111.08m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_028_level_ii_response_s() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var chargeResponse = _creditService.Charge(111.09m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_029_level_ii_no_response() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            var chargeResponse = _creditService.Charge(111.10m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_030_level_ii_no_response() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            var chargeResponse = _creditService.Charge(111.11m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_031_level_ii_no_response() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "750241234" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            var chargeResponse = _creditService.Charge(111.12m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_032_level_ii_no_response() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            var chargeResponse = _creditService.Charge(111.13m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithCpcReq(true)
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
        public void moto_033_offline_sale() {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineCharge(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void moto_033_offline_authorization() {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineAuth(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void moto_034_offline_credit_return() {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.Refund(15.15m)
                .WithCard(card)
                .WithDirectMarketData(directMarketData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ONLINE VOID / REVERSAL

        [TestMethod, Ignore]
        public void moto_035_void_moto_10() {
            var voidResponse = _creditService.Void(test10TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        public void moto_036_void_moto_20() {
            var voidResponse = _creditService.Void(test20TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        // ONE CARD - GSB CARD FUNCTIONS

        // BALANCE INQUIRY

        [TestMethod, Ignore]
        public void moto_037_balance_inquiry_gsb() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard { Number = "6277220572999800", ExpYear = 2049, ExpMonth = 12 };

            var response = _creditService.PrePaidBalanceInquiry()
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void moto_038_add_value_gsb() {
            var trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?"
            };

            var response = _creditService.PrePaidAddValue(15.00m)
                .WithTrackData(trackData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // SALE

        [TestMethod, Ignore]
        public void moto_039_charge_gsb() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };

            var card = new HpsCreditCard { Number = "6277220572999800", ExpYear = 2049, ExpMonth = 12 };

            var chargeResponse = _creditService.Charge(2.05m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(new HpsDirectMarketData { InvoiceNumber = "123456" })
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            test39TransactionId = chargeResponse.TransactionId;
        }

        [TestMethod, Ignore]
        public void moto_040_charge_gsb() {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860", Zip = "75024" } };
            var card = new HpsCreditCard { Number = "6277220572999800", ExpYear = 2049, ExpMonth = 12 };

            var chargeResponse = _creditService.Charge(2.10m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(new HpsDirectMarketData { InvoiceNumber = "123456" })
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // ONLINE VOID / REVERSAL

        [TestMethod, Ignore]
        public void moto_041_void_gsb() {
            var voidResponse = _creditService.Void(test39TransactionId)
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        // HMS GIFT - REWARDS

        // ACTIVATE

        [TestMethod]
        public void moto_042_activate_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Activate(6.00m).WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_043_activate_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Activate(7.00m).WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // LOAD / ADD VALUE

        [TestMethod]
        public void moto_044_add_value_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Activate(8.00m).WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_045_add_value_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Activate(8.00m).WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // BALANCE INQUIRY

        [TestMethod]
        public void moto_046_balance_inquiry_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Balance().WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void moto_047_balance_inquiry_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Balance().WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void moto_048_replace_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Replace().WithOldCard(giftCard1).WithNewCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void moto_049_replace_gift_2() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Replace().WithOldCard(giftCard2).WithNewCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // SALE / REDEEM

        [TestMethod]
        public void moto_050_sale_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Sale(1.0m).WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_051_sale_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Sale(2.0m).WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_052_sale_gift_1_void() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var saleResponse = _giftService.Sale(3.0m)
                .WithCard(giftCard1)
                .Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("0", saleResponse.ResponseCode);
            test52TransactionId = saleResponse.TransactionId;
        }

        [TestMethod]
        public void moto_053_sale_gift_2_reversal() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var saleResponse = _giftService.Sale(4.0m).WithCard(giftCard2).Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("0", saleResponse.ResponseCode);
            test53TransactionId = saleResponse.TransactionId;
        }

        // VOID

        [TestMethod, Ignore]
        public void moto_054_void_gift() {
            var voidResponse = _giftService.VoidSale(test52TransactionId).Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("0", voidResponse.ResponseCode);
        }

        // REVERSAL

        [TestMethod, Ignore]
        public void moto_055_reversal_gift() {
            var reverseResponse = _giftService.Reverse(4.0m).WithTransactionId(test53TransactionId).Execute();

            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("0", reverseResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        public void moto_056_reversal_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Reverse(2.0m)
                .WithCard(giftCard2)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void moto_057_deactivate_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Deactivate().WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // moto_058_receipts_messaging: print and scan receipt for test 51

        // BALANCE INQUIRY

        [TestMethod]
        public void moto_059_balance_inquiry_rewards_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Balance().WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        [TestMethod]
        public void moto_060_balance_inquiry_rewards_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Balance().WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        // ALIAS

        [TestMethod]
        public void moto_061_create_alias_gift_1() {
            var response = _giftService.Alias().WithAction(GiftCardAliasReqBlock1TypeAction.CREATE).WithAlias("9725550100").Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_062_create_alias_gift_2() {
            var response = _giftService.Alias().WithAction(GiftCardAliasReqBlock1TypeAction.CREATE).WithAlias("9725550100").Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_063_add_alias_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Alias().WithAction(GiftCardAliasReqBlock1TypeAction.ADD).WithAlias("2145550199")
                .WithCard(giftCard1)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_064_add_alias_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Alias().WithAction(GiftCardAliasReqBlock1TypeAction.ADD).WithAlias("2145550199")
                .WithCard(giftCard2)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_065_delete_alias_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Alias().WithAction(GiftCardAliasReqBlock1TypeAction.DELETE).WithAlias("2145550199")
                .WithCard(giftCard1)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // SALE / REDEEM

        [TestMethod]
        public void moto_066_redeem_points_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Sale(100.00m).WithCard(giftCard1)
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_067_redeem_points_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Sale(200.00m).WithCard(giftCard2)
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_068_redeem_points_gift_2() {
            var giftCard = new HpsGiftCard { Value = "9725550100", ValueType = ItemChoiceType.Alias };

            var response = _giftService.Sale(300.00m).WithCard(giftCard)
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // REWARDS

        [TestMethod]
        public void moto_069_rewards_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Reward(10.00m).WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_070_rewards_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Reward(11.00m).WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void moto_071_replace_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Replace().WithOldCard(giftCard1).WithNewCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_072_replace_gift_2() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Replace().WithOldCard(giftCard2).WithNewCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void moto_073_deactivate_gift_1() {
            var giftCard1 = new HpsGiftCard { Value = "5022440000000000098" };
            var response = _giftService.Deactivate().WithCard(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void moto_074_deactivate_gift_2() {
            var giftCard2 = new HpsGiftCard { Value = "5022440000000000007" };
            var response = _giftService.Deactivate().WithCard(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // moto_075_receipts_messaging: print and scan receipt for test 51

        // CLOSE BATCH

        [TestMethod]
        public void moto_999_CloseBatch()
        {
            try
            {
                var response = _batchService.CloseBatch();
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
