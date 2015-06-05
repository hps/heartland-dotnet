using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services.Batch;
using SecureSubmit.Services.Credit;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Tests
{
    [TestClass]
    public class FluentTests
    {
        [TestMethod]
        public void Batch_WhenOpen_ShouldClose()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            creditSvc.Charge(50).WithCard(TestCreditCard.ValidAmex).AllowDuplicates().Execute();
            
            var batchSvc = new HpsBatchService(TestServicesConfig.ValidSecretKeyConfig());
            var response = batchSvc.Close().WithClientTransactionId(12345).Execute();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Amex_WhenCardIsOk_ShouldCharge()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Charge(50).WithCard(TestCreditCard.ValidAmex).AllowPartialAuth(true).WithClientTransactionId(12345).Execute();
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Amex_WhenCardIsOk_ShouldAuthorize()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50).WithCard(TestCreditCard.ValidAmex).Execute();
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        [ExpectedException(typeof(HpsCreditException))]
        public void Amex_ResponseCode_ShouldIndicateDenied()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            creditSvc.Charge(10.08m).WithCard(TestCreditCard.ValidAmex).Execute();
        }

        [TestMethod]
        public void Visa_Capture_ShouldReturnOk()
        {
            var random = new Random();
            var randomNumber = random.Next(10, 100);

            // Authorize the card.
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var authResponse = creditSvc.Authorize(randomNumber)
                .WithCard(TestCreditCard.ValidVisa)
                .WithCardHolder(TestCardHolder.ValidCardHolder)
                .Execute();

            Assert.AreEqual("00", authResponse.ResponseCode);

            // Capture the authorization.
            var captureResponse = creditSvc.Capture(authResponse.TransactionId).Execute();
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
    }
}
