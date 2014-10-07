// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmexTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   AMEX unit tests.
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

    /// <summary>MasterCard unit tests.</summary>
    [TestClass]
    public class AmexTests
    {
        /// <summary>The AMEX is ok test method.</summary>
        [TestMethod]
        public void Amex_WhenCardIsOk_ShouldReturnValidResult()
        {
            var response = ChargeValidAmex(50);
            StringAssert.Matches(response.ResponseCode, new Regex("00"));
        }

        /// <summary>The AMEX test transaction details.</summary>
        [TestMethod]
        public void Amex_WhenCardIsOkAndIncludesDetails_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(50, "usd", TestCreditCard.ValidAmex,
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
        }

        #region AVS Tests

        /// <summary>AVS result code should be "A" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualA()
        {
            var response = ChargeValidAmex(90.01M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^A$"));
        }

        /// <summary>AVS result code should be "N" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualN()
        {
            var response = ChargeValidAmex(90.02M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^N$"));
        }

        /// <summary>AVS result code should be "R" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualR()
        {
            var response = ChargeValidAmex(90.03M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^R$"));
        }

        /// <summary>AVS result code should be "S" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualS()
        {
            var response = ChargeValidAmex(90.04M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^S$"));
        }

        /// <summary>AVS result code should be "U" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualU()
        {
            var response = ChargeValidAmex(90.05M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^U$"));
        }

        /// <summary>AVS result code should be "W" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualW()
        {
            var response = ChargeValidAmex(90.06M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^W$"));
        }

        /// <summary>AVS result code should be "X" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualX()
        {
            var response = ChargeValidAmex(90.07M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^X$"));
        }

        /// <summary>AVS result code should be "Y" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualY()
        {
            var response = ChargeValidAmex(90.08M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Y$"));
        }

        /// <summary>AVS result code should be "Z" test method.</summary>
        [TestMethod]
        public void Amex_AvsResultCode_ShouldEqualZ()
        {
            var response = ChargeValidAmex(90.09M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Z$"));
        }

        #endregion

        #region CVV Tests

        /// <summary>CVV result code should be "M" test method.</summary>
        [TestMethod]
        public void Amex_CvvResultCode_ShouldEqualM()
        {
            var response = ChargeValidAmex(97.01M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^M$"));
        }

        /// <summary>CVV result code should be "N" test method.</summary>
        [TestMethod]
        public void Amex_CvvResultCode_ShouldEqualN()
        {
            var response = ChargeValidAmex(97.02M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^N$"));
        }

        /// <summary>CVV result code should be "P" test method.</summary>
        [TestMethod]
        public void Amex_CvvResultCode_ShouldEqualP()
        {
            var response = ChargeValidAmex(97.03M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^P$"));
        }

        #endregion

        #region Amex to Visa 2nd

        /// <summary>Transaction response code should indicate denied (ResponseText: 'DECLINE', ResponseCode: '51').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateDenied()
        {
            try
            {
                ChargeValidAmex(10.08M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate card expired (ResponseText: 'EXPIRED CARD', ResponseCode: '54').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateCardExpired()
        {
            try
            {
                ChargeValidAmex(10.32M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ExpiredCard, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate please call (ResponseText: 'CALL', ResponseCode: '02').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicatePleaseCall()
        {
            try
            {
                ChargeValidAmex(10.34M);
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
        public void Amex_ResponseCode_ShouldIndicateInvalidMerchant()
        {
            try
            {
                ChargeValidAmex(10.22M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid amount (ResponseText: 'AMOUNT ERROR', ResponseCode: '13').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateInvalidAmount()
        {
            try
            {
                ChargeValidAmex(10.27M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.InvalidAmount, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid account (ResponseText: 'NO ACTION TAKEN', ResponseCode: '76').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateNoActionTaken()
        {
            try
            {
                ChargeValidAmex(10.14M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid CVV2 (ResponseText: 'CVV2 MISMATCH', ResponseCode: 'N7').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateInvalidCvv2()
        {
            try
            {
                ChargeValidAmex(10.23M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectCvc, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate message format error (ResponseText: 'CID FORMAT ERROR', ResponseCode: 'EC').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateMessageFormatError()
        {
            try
            {
                ChargeValidAmex(10.06M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid originator (ResponseText: 'SERV NOT ALLOWED', ResponseCode: '58').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateInvalidOriginator()
        {
            try
            {
                ChargeValidAmex(10.30M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate card declined (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateCardDeclined()
        {
            try
            {
                ChargeValidAmex(10.25M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate account cancelled (ResponseText: 'NO ACCOUNT', ResponseCode: '78').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateAccountCancelled()
        {
            try
            {
                ChargeValidAmex(10.13M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate merchant close (ResponseText: 'ERROR', ResponseCode: '06').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicateMerchantClose()
        {
            try
            {
                ChargeValidAmex(10.12M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate pick up card (ResponseText: 'HOLD-CALL', ResponseCode: '44').</summary>
        [TestMethod]
        public void Amex_ResponseCode_ShouldIndicatePickUpCard()
        {
            try
            {
                ChargeValidAmex(10.04M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        #endregion

        #region Verify, Authorize & Capture

        /// <summary>Amex verify should return response code '85'.</summary>
        [TestMethod]
        public void Amex_Verify_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Verify(TestCreditCard.ValidAmex, TestCardHolder.ValidCardHolder, true);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Amex authorize should return response code '00'.</summary>
        [TestMethod]
        public void Amex_Authorize_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidAmex, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Amex authorize should return response code '00'.</summary>
        [TestMethod]
        public void Amex_AuthorizeAndRequestToken_ShouldGetTokenAndReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidAmex, TestCardHolder.ValidCardHolder, true);
            Assert.AreEqual(0, response.TokenData.TokenRspCode);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Amex authorize should return response code '00'.</summary>
        [TestMethod]
        public void Amex_Capture_ShouldReturnOk()
        {
            // Authorize the card.
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var authResponse = creditSvc.Authorize(50, "usd", TestCreditCard.ValidAmex, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // Capture the authorization.
            var captureResponse = creditSvc.Capture(authResponse.TransactionId);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        /// <summary>Amex void test.</summary>
        [TestMethod]
        public void Amex_ShouldVoid_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var creditResponse = creditSvc.Charge(25.00m, "usd", TestCreditCard.ValidAmex, TestCardHolder.CertCardHolderShortZipNoStreet);
            var voidResponse = creditSvc.Void(creditResponse.TransactionId);
            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^00$"));
        }

        #endregion

        #region Card Present

        [TestMethod]
        public void Amex_WhenValidTrackData_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", new HpsTrackData
            {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Mehod = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        #endregion

        /// <summary>Charge an AMEX with a valid config and valid AMEX info.</summary>
        /// <param name="amt">Amount to charge</param>
        /// <returns>The HPS Charge.</returns>
        private static HpsCharge ChargeValidAmex(decimal amt)
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Charge(amt, "usd", TestCreditCard.ValidAmex, TestCardHolder.ValidCardHolder);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            return response;
        }
    }
}
// ReSharper restore InconsistentNaming