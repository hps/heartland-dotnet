// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Discover unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SecureSubmit.Services.Credit;

// ReSharper disable InconsistentNaming
namespace SecureSubmit.Tests
{
    using Entities;
    using Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.RegularExpressions;
    using TestData;

    /// <summary>Discover unit tests.</summary>
    [TestClass]
    public class DiscoverTests
    {
        /// <summary>The Discover is ok test method.</summary>
        [TestMethod]
        public void Discover_WhenCardIsOk_ShouldReturnValidResult()
        {
            var response = ChargeValidDiscover(50);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>The Discover test transaction details.</summary>
        [TestMethod]
        public void Descover_WhenCardIsOkAndIncludesDetails_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(50, "usd", TestCreditCard.ValidDiscover,
                TestCardHolder.ValidCardHolder, false, "descriptor", true, new HpsTransactionDetails
                {
                    Memo = "memo",
                    InvoiceNumber = "1234",
                    CustomerId = "customerID",
                    ClientTransactionId = 12345678
                });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var transaction = service.Get(charge.TransactionId);
            Assert.IsNotNull(transaction);
            StringAssert.Matches(transaction.Memo, new Regex("memo"));
            StringAssert.Matches(transaction.InvoiceNumber, new Regex("1234"));
            StringAssert.Matches(transaction.CustomerId, new Regex("customerID"));
            Assert.AreEqual(charge.ClientTransactionId, 12345678);
        }

        #region AVS Tests

        /// <summary>AVS result code should be "A" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualA()
        {
            var response = ChargeValidDiscover(91.01M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^A$"));
        }

        /// <summary>AVS result code should be "N" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualN()
        {
            var response = ChargeValidDiscover(91.02M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^N$"));
        }

        /// <summary>AVS result code should be "R" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualR()
        {
            var response = ChargeValidDiscover(91.03M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^R$"));
        }

        /// <summary>AVS result code should be "U" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualU()
        {
            var response = ChargeValidDiscover(91.05M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^U$"));
        }

        /// <summary>AVS result code should be "Y" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualY()
        {
            var response = ChargeValidDiscover(91.06M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Y$"));
        }

        /// <summary>AVS result code should be "Z" test method.</summary>
        [TestMethod]
        public void Discover_AvsResultCode_ShouldEqualZ()
        {
            var response = ChargeValidDiscover(91.07M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Z$"));
        }

        #endregion

        #region Discover to Visa 2nd

        /// <summary>Transaction response code should indicate refer card issuer (ResponseText: 'CALLS', ResponseCode: '02').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateReferCardIssuer()
        {
            try
            {
                ChargeValidDiscover(10.34M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid merchant (ResponseText: 'TERM ID ERROR', ResponseCode: '03').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidMerchant()
        {
            try
            {
                ChargeValidDiscover(10.22M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate pick up card (ResponseText: 'HOLD-CALLS', ResponseCode: '44').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicatePickUpCard()
        {
            try
            {
                ChargeValidDiscover(10.04M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate do not honor (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateDoNotHonor()
        {
            try
            {
                ChargeValidDiscover(10.25M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid transaction (ResponseText: 'INVALID TRANS', ResponseCode: '12').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidTransaction()
        {
            try
            {
                ChargeValidDiscover(10.26M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid amount (ResponseText: 'AMOUNT ERROR', ResponseCode: '13').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidAmount()
        {
            try
            {
                ChargeValidDiscover(10.27M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.InvalidAmount, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid card (ResponseText: 'CARD NO. ERROR', ResponseCode: '14').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidCard()
        {
            try
            {
                ChargeValidDiscover(10.28M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid issuer (ResponseText: 'NO SUCH ISSUER', ResponseCode: '15').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidIssuer()
        {
            try
            {
                ChargeValidDiscover(10.18M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate system error re-enter (ResponseText: 'RE ENTER', ResponseCode: '19').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateSystemErrorReenter()
        {
            try
            {
                ChargeValidDiscover(10.29M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate message format error (ResponseText: 'CID FORMAT ERROR', ResponseCode: 'EC').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateMessageFormatError()
        {
            try
            {
                ChargeValidDiscover(10.06M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate lost card (ResponseText: 'HOLD-CALL', ResponseCode: '41').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateLostCard()
        {
            try
            {
                ChargeValidDiscover(10.31M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate insufficient funds (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInsufficientFunds()
        {
            try
            {
                ChargeValidDiscover(10.08M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate no saving account (ResponseText: 'NO SAVE ACCOUNT', ResponseCode: '53').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateNoSavingAccount()
        {
            try
            {
                ChargeValidDiscover(10.17M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate expired card (ResponseText: 'EXPIRED CARD', ResponseCode: '54').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateExpiredCard()
        {
            try
            {
                ChargeValidDiscover(10.32M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ExpiredCard, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate no card record (ResponseText: 'INVALID TRANS', ResponseCode: '56').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateNoCardRecord()
        {
            try
            {
                ChargeValidDiscover(10.24M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate transaction not permitted on card (ResponseText: 'SERV NOT ALLOWED', ResponseCode: '57').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateTxnNotPermittedOnCard()
        {
            try
            {
                ChargeValidDiscover(10.20M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid acquirer (ResponseText: 'SERV NOT ALLOWED', ResponseCode: '58').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidAcquirer()
        {
            try
            {
                ChargeValidDiscover(10.30M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds limit (ResponseText: 'DECLINE', ResponseCode: '61').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateExceedsLimit()
        {
            try
            {
                ChargeValidDiscover(10.09M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate restricted card (ResponseText: 'DECLINE', ResponseCode: '62').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateRestrictedCard()
        {
            try
            {
                ChargeValidDiscover(10.10M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate security violation (ResponseText: 'SEC VIOLATION', ResponseCode: '63').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateSecurityViolation()
        {
            try
            {
                ChargeValidDiscover(10.19M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds frequency limit (ResponseText: 'DECLINE$', ResponseCode: '65').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateExceedsFreqLimit()
        {
            try
            {
                ChargeValidDiscover(10.11M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate no to account (ResponseText: 'NO ACCOUNT', ResponseCode: '78').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateNoToAccount()
        {
            try
            {
                ChargeValidDiscover(10.13M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid account (ResponseText: 'CARD NO. ERROR', ResponseCode: '14').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateInvalidAccount()
        {
            try
            {
                ChargeValidDiscover(10.14M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        ///// <summary>Transaction response code should indicate switch not available (ResponseText: 'NO REPLY', ResponseCode: '14').</summary>
        //[TestMethod]
        //public void Discover_ResponseCode_ShouldIndicateSwitchNotAvailable()
        //{
        //    try
        //    {
        //        ChargeValidDiscover(10.33M);
        //    }
        //    catch (HpsCreditException e)
        //    {
        //        Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
        //        return;
        //    }

        //    Assert.Fail("No exception was thrown.");
        //}

        /// <summary>Transaction response code should indicate system error (ResponseText: 'SYSTEM ERROR', ResponseCode: '96').</summary>
        [TestMethod]
        public void Discover_ResponseCode_ShouldIndicateSystemError()
        {
            try
            {
                ChargeValidDiscover(10.21M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        #endregion

        #region Verify, Authorize, Capture & Void

        /// <summary>Discover verify should return response code '85'.</summary>
        [TestMethod]
        public void Discover_Verify_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Verify(TestCreditCard.ValidDiscover, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("85", response.ResponseCode);
        }

        /// <summary>Discover authorize should return response code '00'.</summary>
        [TestMethod]
        public void Discover_Authorize_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidDiscover, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Discover authorize should return response code '00'.</summary>
        [TestMethod]
        public void Discover_AuthorizeAndRequestToken_ShouldGetTokenAndReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidDiscover, TestCardHolder.ValidCardHolder, true);
            Assert.AreEqual(0, response.TokenData.TokenRspCode);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Discover authorize should return response code '00'.</summary>
        [TestMethod]
        public void Discover_Capture_ShouldReturnOk()
        {
            // Authorize the card.
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var authResponse = creditSvc.Authorize(50, "usd", TestCreditCard.ValidDiscover, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // Capture the authorization.
            var captureResponse = creditSvc.Capture(authResponse.TransactionId);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        /// <summary>MasterCard void test.</summary>
        [TestMethod]
        public void Discover_ShouldVoid_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var creditResponse = creditSvc.Charge(25.00m, "usd", TestCreditCard.ValidDiscover, TestCardHolder.CertCardHolderShortZipNoStreet);
            var voidResponse = creditSvc.Void(creditResponse.TransactionId);
            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^00$"));
        }

        #endregion

        #region Card Present

        [TestMethod]
        public void Discover_WhenValidSwipeTrackData_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Mehod = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Discover_WhenValidProximityTrackData_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25122011000062111401?;6011000990156527=25122011000062111401?",
                Mehod = HpsTrackDataMethod.Proximity
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        #endregion

        /// <summary>Charge a Discover with a valid config and valid Discover info.</summary>
        /// <param name="amt">Amount to charge</param>
        /// <returns>The HPS Charge.</returns>
        private static HpsCharge ChargeValidDiscover(decimal amt)
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Charge(amt, "usd", TestCreditCard.ValidDiscover, TestCardHolder.ValidCardHolder);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            return response;
        }
    }
}
// ReSharper restore InconsistentNaming