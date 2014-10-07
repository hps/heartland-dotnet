// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Certification unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SecureSubmit.Services.Batch;
using SecureSubmit.Services.Credit;

namespace SecureSubmit.Tests
{
    using Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services;
    using System.Text.RegularExpressions;
    using TestData;

    /// <summary>SDK certification unit tests.</summary>
    [TestClass]
    public class CertTests
    {
        // If using a transaction descriptor for certification, set it here. If not, set to null.
        private const string TestDescriptor = "Heartland, Inc.";

        // Set certConfig to the services configuration you'd like to use for certification.
        private readonly HpsServicesConfig _certConfig = TestServicesConfig.ValidSecretKeyConfig();

        /// <summary>Run this test to certify the SDK (tests must be run serially).</summary>
        [TestMethod]
        public void Cert_ShouldRun_Ok()
        {
            Batch_ShouldClose_Ok();
            Visa_ShouldCharge_Ok();
            MasterCard_ShouldCharge_Ok();
            Discover_ShouldCharge_Ok();
            Amex_ShouldCharge_Ok();
            Jcb_ShouldCharge_Ok();
            Visa_ShouldVerify_Ok();
            MasterCard_ShouldVerify_Ok();
            Discover_ShouldVerify_Ok();
            Amex_Avs_ShouldBe_Ok();
            Mastercard_Return_ShouldBe_Ok();
            Visa_ShouldReverse_Ok();
            Batch_ShouldClose_Ok();
        }

        /// <summary>Batch close cert test.</summary>
        [TestMethod]
        public void Batch_ShouldClose_Ok()
        {
            try
            {
                var batchSvc = new HpsBatchService(_certConfig);
                var response = batchSvc.CloseBatch();
                Assert.IsNotNull(response);
            }
            catch (HpsGatewayException e)
            {
                if (e.Code != HpsExceptionCodes.NoOpenBatch && e.Code != HpsExceptionCodes.UnknownCreditError)
                {
                    Assert.Fail("Something failed other than 'no open batch'.");
                }
            }
        }

        /// <summary>VISA charge cert test.</summary>
        [TestMethod]
        public void Visa_ShouldCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Charge(17.01m, "usd", TestCreditCard.ValidVisa,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>MasterCard charge cert test.</summary>
        [TestMethod]
        public void MasterCard_ShouldCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Charge(17.02m, "usd", TestCreditCard.ValidMasterCard,
                TestCardHolder.CertCardHolderShortZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Discover charge cert test.</summary>
        [TestMethod]
        public void Discover_ShouldCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Charge(17.03m, "usd", TestCreditCard.ValidDiscover,
                TestCardHolder.CertCardHolderLongZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Amex charge cert test.</summary>
        [TestMethod]
        public void Amex_ShouldCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Charge(17.04m, "usd", TestCreditCard.ValidAmex,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>JCB charge cert test.</summary>
        [TestMethod]
        public void Jcb_ShouldCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Charge(17.05m, "usd", TestCreditCard.ValidJcb,
                TestCardHolder.CertCardHolderLongZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>VISA verify cert test.</summary>
        [TestMethod]
        public void Visa_ShouldVerify_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Verify(TestCreditCard.ValidVisa);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^85$"));
        }

        /// <summary>MasterCard verify cert test.</summary>
        [TestMethod]
        public void MasterCard_ShouldVerify_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Verify(TestCreditCard.ValidMasterCard);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^85$"));
        }

        /// <summary>Discover verify cert test.</summary>
        [TestMethod]
        public void Discover_ShouldVerify_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Verify(TestCreditCard.ValidDiscover);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^85$"));
        }

        /// <summary>Amex AVS cert test.</summary>
        [TestMethod]
        public void Amex_Avs_ShouldBe_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Verify(TestCreditCard.ValidAmex, TestCardHolder.CertCardHolderShortZipNoStreet);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Mastercard return test.</summary>
        [TestMethod]
        public void Mastercard_Return_ShouldBe_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Refund(15.15m, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderShortZip);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>VISA verify cert test.</summary>
        [TestMethod]
        public void Visa_ShouldReverse_Ok()
        {
            var chargeSvc = new HpsCreditService(_certConfig);
            var response = chargeSvc.Reverse(17.01m, "usd", TestCreditCard.ValidVisa);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }
    }
}
