using System;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Tests {

    [TestClass]
    public class GeneralTests {

        private static readonly string TodayDate = DateTime.Today.ToString("yyyyMMdd");
        private static readonly string IdentifierBase = "{0}-{1}" + Guid.NewGuid().ToString().Substring(0, 10);
        HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };

        [TestMethod]
        public void CvvWithLeadingZeros() {
            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "012"
            };

            var creditService = new HpsCreditService(ServicesConfig);
            var response = creditService.Charge(15.15m, "usd", card);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void RefundWithTrackData() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            var creditService = new HpsCreditService(ServicesConfig);
            HpsRefund response = creditService.Refund(12.00M, "USD", trackData);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void AddPaymentMethodWithToken()
        {
            // Create Customer
            var customer = new HpsPayPlanCustomer
            {
                CustomerIdentifier = GetIdentifier("Person"),
                FirstName = "John",
                LastName = "Doe",
                CustomerStatus = HpsPayPlanCustomerStatus.Active,
                PrimaryEmail = "john.doe@email.com",
                AddressLine1 = "123 Main St",
                City = "Dallas",
                StateProvince = "TX",
                ZipPostalCode = "98765",
                Country = "USA",
                PhoneDay = "5551112222"
            };
            HpsPayPlanService payPlanService = new HpsPayPlanService(TestServicesConfig.ValidTokenServiceConfig());
            var response = payPlanService.AddCustomer(customer);
            var customerKey = response.CustomerKey;

            // Create Card & Token
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvv = "123"
            };
            var tokenService = new HpsTokenService("pkapi_cert_jKc1FtuyAydZhZfbB3");
            var tokenResponse = tokenService.GetToken(card);

            // Create & Add Payment via Token
            var newPaymentMethod = new HpsPayPlanPaymentMethod
            {
                CustomerKey = customerKey,
                NameOnAccount = "Bill Johnson",
                PaymentToken = tokenResponse.token_value,
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                Country = "USA"
            };

            var result = payPlanService.AddPaymentMethod(newPaymentMethod);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PaymentMethodKey);
        }

        [TestMethod]
        public void GetTransactionHasTxnStatus()
        {
            // Make transaction
            var card = new HpsCreditCard
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "012"
            };

            var creditService = new HpsCreditService(ServicesConfig);
            var response = creditService.Charge(15.15m, "usd", card);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // Get details
            var details = creditService.Get(response.TransactionId);
            Assert.IsNotNull(details);
            Assert.IsNotNull(details.TransactionStatus);
            Assert.AreNotEqual("", details.TransactionStatus);
        }

        private static string GetIdentifier(string identifier)
        {
            var rValue = string.Format(IdentifierBase, TodayDate, identifier);
            Console.WriteLine(rValue);

            return rValue;
        }

        [TestMethod]
        public void CreditSaleWithTokenExpiry() {
            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Charge(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, null, 12, 2025);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditAuthWithTokenExpiry() {
            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Authorize(10m, "usd", token_reponse.token_value, null, false, null, false, null, 0, 12, 2025);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerifyWithTokenExpiry() {
            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2014,
                Cvv = "123"
            };

            var tokenService = new HpsTokenService("pkapi_cert_m0e9bI2WbBHk0ALyQL");
            var token_reponse = tokenService.GetToken(card);

            var creditService = new HpsCreditService(new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });
            var response = creditService.Verify(token_reponse.token_value, null, false, null, 12, 2025);
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }
    }
}