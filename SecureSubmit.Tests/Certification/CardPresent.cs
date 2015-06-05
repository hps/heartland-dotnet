using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Batch;
using SecureSubmit.Services.Credit;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Tests.Certification
{
    [TestClass]
    public class CardPresent
    {
        // If using a transaction descriptor for certification, set it here. If not, set to null.
        private const string TestDescriptor = "Heartland, Inc.";

        // Set certConfig to the services configuration you'd like to use for certification.
        private readonly HpsServicesConfig _certConfig = TestServicesConfig.ValidSecretKeyConfig();

        [TestMethod]
        public void Cert_ShouldRun_Ok()
        {
            Batch_ShouldClose();
            Visa_Swipe_ShouldVerify();
            Mastercard_Swipe_ShouldVerify();
            Discover_Swipe_ShouldVerify();
            Amex_Manual_ShouldVerify();
            Visa_Swipe_ShouldCharge();
            Mastercard_Swipe_ShouldCharge();
            Discover_Swipe_ShouldCharge();
            Amex_Swipe_ShouldCharge();
        }

        [TestMethod]
        public void Batch_ShouldClose()
        {
            try
            {
                var batchSvc = new HpsBatchService(_certConfig);
                var response = batchSvc.CloseBatch();
                Assert.IsNotNull(response);
            }
            catch (HpsGatewayException e)
            {
                if (e.Code != HpsExceptionCodes.NoOpenBatch && e.Code != HpsExceptionCodes.UnknownIssuerError)
                {
                    Assert.Fail("Something failed other than 'no open batch'.");
                }
            }
        }

        /* ----- CARD VERIFY ----- */

        [TestMethod]
        public void Visa_Swipe_ShouldVerify()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var verify = service.Verify(new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(verify);
            StringAssert.Matches(verify.ResponseCode, new Regex("85"));
        }

        [TestMethod]
        public void Mastercard_Swipe_ShouldVerify()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var verify = service.Verify(new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(verify);
            StringAssert.Matches(verify.ResponseCode, new Regex("85"));
        }

        [TestMethod]
        public void Discover_Swipe_ShouldVerify()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var verify = service.Verify(new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(verify);
            StringAssert.Matches(verify.ResponseCode, new Regex("85"));
        }

        [TestMethod]
        public void Amex_Manual_ShouldVerify()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Verify(TestCreditCard.ValidAmex, TestCardHolder.CertCardHolderShortZip, true);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /* ----- SALE (FOR MULTI-USE TOKEN) ----- */

        [TestMethod]
        public void Visa_Swipe_ShouldChargeWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.01m, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0m, false, true, null, "", true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Swipe_ShouldChargeWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.02m, "usd", new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0m, false, true, null, "", true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Discover_Swipe_ShouldChargeWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.03m, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0m, false, true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Amex_Swipe_ShouldChargeWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.04m, "usd", new HpsTrackData
            {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0m, false, true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        /* ----- SALE (SWIPED) ----- */

        [TestMethod]
        public void Visa_Swipe_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.01m, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0M, false, false, null, "", true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Swipe_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.02m, "usd", new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0M, false, false, null, "", true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Discover_Swipe_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.03m, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            }, null, 0M, false, false, null, "", true);

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Amex_Swipe_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.04m, "usd", new HpsTrackData
            {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }


        [TestMethod]
        public void DiscoverJcb_Swipe_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.04m, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_Swipe_ShouldChargeVoid()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(15.06m, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        /* ----- SALE (MANUAL CARD PRESENT) ----- */

        [TestMethod]
        public void Visa_PresentManual_ShouldCharge_Ok()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(16.01m, "usd", TestCreditCard.ValidVisa,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void MasterCard_PresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(16.02m, "usd", TestCreditCard.ValidMasterCard,
                TestCardHolder.CertCardHolderShortZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Discover_PresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(16.03m, "usd", TestCreditCard.ValidDiscover,
                TestCardHolder.CertCardHolderLongZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Amex_PresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(16.04m, "usd", TestCreditCard.ValidAmex,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Jcb_PresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(16.05m, "usd", TestCreditCard.ValidJcb,
                TestCardHolder.CertCardHolderLongZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /* ----- SALE (MANUAL CARD NOT PRESENT) ----- */

        [TestMethod]
        public void Visa_NotPresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(17.01m, "usd", TestCreditCard.ValidVisa,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void MasterCard_NotPresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(17.02m, "usd", TestCreditCard.ValidMasterCard,
                TestCardHolder.CertCardHolderShortZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Discover_NotPresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(17.03m, "usd", TestCreditCard.ValidDiscover,
                TestCardHolder.CertCardHolderLongZipNoStreet, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Amex_NotPresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(17.04m, "usd", TestCreditCard.ValidAmex,
                TestCardHolder.CertCardHolderShortZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Jcb_NotPresentManual_ShouldCharge()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = service.Charge(17.05m, "usd", TestCreditCard.ValidJcb,
                TestCardHolder.CertCardHolderLongZip, false, TestDescriptor);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /* ----- AUTHORIZATION DELAYED CAPTURE ----- */

        [TestMethod]
        public void Visa_Swipe_ShouldAuthorizeAndCaptureWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(15.08m, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(auth);
            StringAssert.Matches(auth.ResponseCode, new Regex("00"));

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Swipe_ShouldAuthorizeAndCaptureWithTokenReq()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(15.09m, "usd", new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(auth);
            StringAssert.Matches(auth.ResponseCode, new Regex("00"));

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        /* ----- AUTHORIZATION (DELAYED CAPTURE) ----- */

        [TestMethod]
        public void Visa_ManualPresent_ShouldAuthorizeAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(16.08m, "usd", TestCreditCard.ValidVisa, TestCardHolder.CertCardHolderStreetZipOnly);
            Assert.AreEqual("00", auth.ResponseCode);

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_ManualPresent_ShouldAuthorizeAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(16.09m, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderStreetNumberZipOnly);
            Assert.AreEqual("00", auth.ResponseCode);

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_ManualNotPresent_ShouldAuthorizeAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(17.08m, "usd", TestCreditCard.ValidVisa, TestCardHolder.CertCardHolderStreetZipOnly);
            Assert.AreEqual("00", auth.ResponseCode);

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_ManualNotPresent_ShouldAuthorizeAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(17.09m, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderStreetNumberZipOnly);
            Assert.AreEqual("00", auth.ResponseCode);

            var capture = service.Capture(auth.TransactionId, null);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        /* ----- PARTIALLY APPROVED SALE ----- */

        [TestMethod]
        public void Discover_Swipe_ShouldPartiallyApproveAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(40, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(auth);
            StringAssert.Matches(auth.ResponseCode, new Regex("00"));

            var capture = service.Capture(auth.TransactionId, 40);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_Swipe_ShouldPartiallyApproveAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(110, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(auth);
            StringAssert.Matches(auth.ResponseCode, new Regex("00"));

            var capture = service.Capture(auth.TransactionId, 130);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Manual_ShouldPartiallyApproveAndCapture()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var auth = service.Authorize(65, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderShortZip);
            Assert.AreEqual("00", auth.ResponseCode);

            var capture = service.Capture(auth.TransactionId, 145);

            Assert.IsNotNull(capture);
            StringAssert.Matches(capture.ResponseCode, new Regex("00"));
        }
    }
}
