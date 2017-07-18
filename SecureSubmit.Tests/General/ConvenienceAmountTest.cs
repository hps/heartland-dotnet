using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Services;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Tests.General
{
    [TestClass]
    public class ConvenienceAmountTest
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };
        private readonly HpsFluentCreditService _creditService = new HpsFluentCreditService(ServicesConfig);
        private const bool UseTokens = false;
        private static string masterCardToken;
        private static string visaToken;
        private static long test10TransactionId;
        #region CreditService
        [TestMethod]
        public void CreditAuthWithConvenienceAmt()
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
            var response = creditService.Authorize(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, 12, 2025, 2m);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionDetails = creditService.Get(response.TransactionId);

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(2m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditAuthWithConvenienceAmt_1()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Authorize(10m, "usd", card, null, false, null, false, null, 0, 2m);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            var transactionDetails = creditService.Get(response.TransactionId);

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(2m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditChargeWithConvenienceAmt()
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
            var response = creditService.Charge(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, null, 12, 2025, 2m);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionDetails = creditService.Get(response.TransactionId);

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(2m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditChargeWithConvenienceAmt_1()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Charge(10m, "usd", card, null, false, null, false, null, null, 0, null, 2m);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            var transactionDetails = creditService.Get(response.TransactionId);

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(2m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditOfflineChargeWithConvenienceAmt()
        {
            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.OfflineCharge(10m, "usd", card, "654321", false, false, null, false, null, null, 0, 0, null, 2m);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            var transactionDetails = creditService.Get(response.TransactionId);

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(2m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CreditAuthWithNegativeConvenienceAmt()
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
            var response = creditService.Authorize(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, 12, 2025, -2m);

        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CreditAuthWithNegativeConvenienceAmt_1()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Authorize(10m, "usd", card, null, false, null, false, null, 0, -2m);

        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CreditChargeWithNegativeConvenienceAmt()
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
            var response = creditService.Charge(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, null, 12, 2025, -2m);

        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CreditChargeWithNegativeConvenienceAmt_1()
        {
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Charge(10m, "usd", card, null, false, null, false, null, null, 0, null, -2m);

        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CreditOfflineChargeWithNegativeConvenienceAmt()
        {
            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var creditService = new HpsCreditService(new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.OfflineCharge(10m, "usd", card, "654321", false, false, null, false, null, null, 0, 0, null, -2m);

        }
        #endregion

        #region CreditFluentService    
        [TestMethod]
        public void AuthVisaConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.06m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(10m)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            var transactionDetails = _creditService.Get(authResponse.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void ChargeMasterConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };
            var typeBuilder = _creditService.Charge(17.02m);
            var card = new HpsCreditCard
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var builder = UseTokens ? typeBuilder.WithToken(masterCardToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(10m)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);

            var transactionDetails = _creditService.Get(chargeResponse.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void ChargeVisaConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.01m);
            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var builder = UseTokens ? typeBuilder.WithToken(visaToken) : typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(10m)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            test10TransactionId = chargeResponse.TransactionId;

            var transactionDetails = _creditService.Get(chargeResponse.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void AuthMasterConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(10m)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            var transactionDetails = _creditService.Get(authResponse.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void OfflineAuthorizationConvenienceAmt()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineAuth(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithConvenienceAmt(10m)
                .WithDirectMarketData(directMarketData)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionDetails = _creditService.Get(response.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod]
        public void OfflineSaleConvenienceAmt()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineCharge(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithConvenienceAmt(10m)
                .WithDirectMarketData(directMarketData)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionDetails = _creditService.Get(response.TransactionId).Execute();

            Assert.IsNotNull(transactionDetails);
            Assert.AreEqual(10m, transactionDetails.ConvenienceAmount);
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void AuthVisaNegativeConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.06m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(-10m)
                .WithAllowDuplicates(true)
                .Execute();            
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void ChargeMasterNegativeConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };
            var typeBuilder = _creditService.Charge(17.02m);
            var card = new HpsCreditCard
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var builder = UseTokens ? typeBuilder.WithToken(masterCardToken) :
                typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(-10m)
                .WithAllowDuplicates(true)
                .Execute();            
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void ChargeVisaNegativeConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "75024" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var typeBuilder = _creditService.Charge(17.01m);
            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var builder = UseTokens ? typeBuilder.WithToken(visaToken) : typeBuilder.WithCard(card);

            var chargeResponse = builder
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(-10m)
                .WithAllowDuplicates(true)
                .Execute();            
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void AuthMasterNegativeConvenienceAmt()
        {
            var cardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "6860 Dallas Pkwy", Zip = "750241234" } };
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var authResponse = _creditService.Authorize(17.07m)
                .WithCard(card)
                .WithCardHolder(cardHolder)
                .WithDirectMarketData(directMarketData)
                .WithConvenienceAmt(-10m)
                .WithAllowDuplicates(true)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void OfflineAuthorizationNegativeConvenienceAmt()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineAuth(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithConvenienceAmt(-10m)
                .WithDirectMarketData(directMarketData)
                .WithAllowDuplicates(true)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void OfflineSaleNegativeConvenienceAmt()
        {
            var directMarketData = new HpsDirectMarketData { InvoiceNumber = "123456" };

            var card = new HpsCreditCard
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            var response = _creditService.OfflineCharge(17.10m)
                .WithCard(card)
                .WithOfflineAuthCode("654321")
                .WithConvenienceAmt(-10m)
                .WithDirectMarketData(directMarketData)
                .WithAllowDuplicates(true)
                .Execute();
        }
        #endregion

    }
}
